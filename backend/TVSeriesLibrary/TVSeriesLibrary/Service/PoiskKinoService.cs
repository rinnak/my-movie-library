    using System.Net;
    using System.Text.Json;
    using TVSeriesLibrary.Models;

    namespace TVSeriesLibrary.Service;

    public class PoiskKinoService : IPoiskKinoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PoiskKinoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<MovieCard?> GetRandomMovieAsync()
        {
            var apiKey = _configuration["PoiskKino:ApiKey"];
            
            
            var request = new HttpRequestMessage(
                HttpMethod.Get, "https://api.poiskkino.dev/v1.4/movie/random?notNullFields=name&notNullFields=poster.url");
            
            request.Headers.Add("X-API-KEY", apiKey);
            
            var response = await _httpClient.SendAsync(request);
            

            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine(content);

            // response.EnsureSuccessStatusCode();
            // response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            
            var apiMovie = JsonSerializer.Deserialize<MovieApi>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiMovie == null) return null;
            var movie = new MovieCard()
            {
                Id = apiMovie.Id,
                Name = apiMovie.Name,
                PosterUrl = apiMovie.Poster?.Url ?? "/images/default.jpg",
                Rating = apiMovie.Rating?.Kp ?? apiMovie.Rating?.Imdb,
                Year = apiMovie.Year,
                Genres = apiMovie.Genres != null ? string.Join(", ", apiMovie.Genres.Select(g => g.Name)) : ""
            };
            return movie;
        }

        public async Task<MovieDetails?> GetMovieByIdAsync(int id)
        {
            var apiKey = _configuration["PoiskKino:ApiKey"];
            var request = new HttpRequestMessage(
                HttpMethod.Get, $"https://api.poiskkino.dev/v1.4/movie/{id}");
            
            request.Headers.Add("X-API-KEY", apiKey);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            
            var apiMovie = JsonSerializer.Deserialize<MovieApi>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (apiMovie == null) return null;
            var details = new MovieDetails()
            {
                Id = apiMovie.Id,
                Name = apiMovie.Name ?? "Без названия",
                Year = apiMovie.Year,
                PosterUrl = apiMovie.Poster?.Url ?? "/images/default.jpg",
                Rating = apiMovie.Rating?.Kp ?? apiMovie.Rating?.Imdb,
                Genres = apiMovie.Genres?.Select(g => g.Name).Where(name => name != null).ToList()! ?? [],
                Countries = apiMovie.Countries?.Select(c => c.Name).Where(name => name != null).ToList()! ?? []
            };

            if (apiMovie.Persons != null)
            {
                foreach (var apiPerson in apiMovie.Persons)
                {
                    details.Persons.Add(new PersonInMovieDto
                    {
                        Id = apiPerson.Id,
                        Name = apiPerson.Name ?? apiPerson.EnName,
                        Photo = apiPerson.Photo,
                        Profession = apiPerson.Profession
                    });
                }
            }
            return details;
        }

        public async Task<SearchMovieResponse?> SearchMovieAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new SearchMovieResponse();

            var apiKey = _configuration["PoiskKino:ApiKey"];
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://api.poiskkino.dev/v1.4/movie/search?page=1&limit=10&query={encodedQuery}";
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-API-KEY", apiKey);
            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<SearchMovieResponseRaw>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (apiResponse == null) return null;

            var finalResponse = new SearchMovieResponse()
            {
                Page = apiResponse.Page,
                Limit = apiResponse.Limit,
                Total = apiResponse.Total,
                Pages = apiResponse.Pages,
            };

            foreach (var apiMovie in apiResponse.Docs)
            {
                var card = new MovieCard
                {
                    Id = apiMovie.Id,
                    Name = apiMovie.Name ?? "Без названия",
                    Year = apiMovie.Year,
                    Rating = apiMovie.Rating?.Kp ?? apiMovie.Rating?.Imdb,
                    PosterUrl = apiMovie.Poster?.Url ?? "/images/default.jpg",
                    Genres = apiMovie.Genres != null ? string.Join(", ", apiMovie.Genres.Select(g => g.Name)) : ""
                };
                finalResponse.Docs.Add(card);
            }
            return finalResponse;
        }

        public async Task<List<MovieCard>> GetMoviesByGenreAsync(string genre, int page, int limit)
        {
            if (string.IsNullOrWhiteSpace(genre))
            {
                return [];
            }
            var apiKey = _configuration["PoiskKino:ApiKey"];
            var encodedGenre = Uri.EscapeDataString(genre.ToLower());
            var url = $"https://api.poiskkino.dev/v1.4/movie?page={page}&limit={limit}&selectFields=id&selectFields=name&selectFields=rating&selectFields=poster&selectFields=year&genres.name={encodedGenre}&sortField=rating.kp&sortType=-1&notNullFields=name&notNullFields=poster.url";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-API-KEY", apiKey);
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return [];
            
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<SearchMovieResponseRaw>(json,
                new  JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse == null || apiResponse.Docs == null) return [];
            
            var movies = new List<MovieCard>();
            foreach (var apiMovie in apiResponse.Docs)
            {
                movies.Add(new MovieCard
                {
                    Id = apiMovie.Id,
                    Name = apiMovie.Name ?? "Без названия",
                    Year = apiMovie.Year,
                    Rating = apiMovie.Rating?.Kp ?? apiMovie.Rating?.Imdb,
                    PosterUrl = apiMovie.Poster?.Url ?? "/images/default.jpg",
                    Genres = apiMovie.Genres != null ? string.Join(", ", apiMovie.Genres.Select(g => g.Name)) : ""
                });
            }
            return movies;
        }

        public async Task<MovieDetails> GetHeroMovieAsync()
        {
            var apiKey = _configuration["PoiskKino:ApiKey"];
            var currentYear = DateTime.Now.Year;
            var yearRange = $"{currentYear - 2}-{currentYear}";
            var url = "https://api.poiskkino.dev/v1.4/movie/random" +
                      $"?year={yearRange}" +
                      "&rating.kp=7.5-10" +
                      "&notNullFields=name" +
                      "&notNullFields=poster.url" +
                      "&notNullFields=description" +
                      "&notNullFields=genres.name";
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-API-KEY", apiKey);
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var fallbackUrl = "https://api.poiskkino.dev/v1.4/movie/random" +
                                  "?rating.kp=8.0-10" +
                                  "&notNullFields=name" +
                                  "&notNullFields=poster.url" +
                                  "&notNullFields=description";
                          
                var fallbackRequest = new HttpRequestMessage(HttpMethod.Get, fallbackUrl);
                fallbackRequest.Headers.Add("X-API-KEY", apiKey);
                response = await _httpClient.SendAsync(fallbackRequest);
        
                if (!response.IsSuccessStatusCode) return null;
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var apiMovie = JsonSerializer.Deserialize<MovieApi>(json,
                new  JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var heroMovie = new MovieDetails
            {
                Id = apiMovie.Id,
                Name = apiMovie.Name ?? "Без названия",
                Year = apiMovie.Year,
                PosterUrl = apiMovie.Poster?.Url ?? "/images/default.jpg",
                Rating = apiMovie.Rating?.Kp ?? apiMovie.Rating?.Imdb,
                Description = apiMovie.Description ?? "Описание временно отсутствует.",
                Genres = apiMovie.Genres?.Select(g => g.Name).Where(name => name != null).ToList()! ?? []
            };
            return heroMovie;
        }

        public async Task<List<MovieDetails>> GetThreeHeroMoviesAsync()
        {
            var uniqueMovies = new List<MovieDetails>();
            var seenIds = new HashSet<int>();

            int maxAttempts = 10;
            int attempt = 0;

            while (uniqueMovies.Count < 3 && attempt < maxAttempts)
            {
                attempt++;
                var movieHero =  await GetHeroMovieAsync();
                if (movieHero != null && movieHero.Id.HasValue)
                {
                    if (seenIds.Add(movieHero.Id.Value))
                    {
                        uniqueMovies.Add(movieHero);
                    }
                }
            }

            return uniqueMovies;
        }
        public async Task<List<MovieCard>> GetTopMoviesAsync(
            int page = 1,
            int limit = 14)
        {
            var apiKey = _configuration["PoiskKino:ApiKey"];

            var url =
                $"https://api.poiskkino.dev/v1.4/movie" +
                $"?page={page}" +
                $"&limit={limit}" +
                $"&sortField=votes.kp" +
                $"&sortType=-1" +
                $"&notNullFields=name" +
                $"&notNullFields=poster.url" +
                $"&sortField=rating.kp" +
                $"&sortType=-1";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("X-API-KEY", apiKey);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var apiResponse =
                JsonSerializer.Deserialize<SearchMovieResponseRaw>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (apiResponse == null)
                return [];

            return apiResponse.Docs
                .Select(m => new MovieCard
                {
                    Id = m.Id,
                    Name = m.Name ?? "Без названия",
                    PosterUrl = m.Poster?.Url ?? "/images/default.jpg",
                    Rating = m.Rating?.Kp ?? m.Rating?.Imdb,
                    Year = m.Year,
                    Genres = m.Genres != null ? string.Join(", ", m.Genres.Select(g => g.Name)) : ""
                })
                .ToList();
        }

        public async Task<List<MovieCard>> GetMoviesWithFiltersAsync(
            string? genre,
            string? yearRange,
            string? sortBy,
            int page,
            int limit)
        {
            var apiKey = _configuration["PoiskKino:ApiKey"];
            var url = $"https://api.poiskkino.dev/v1.4/movie?page={page}&limit={limit}&rating.kp=7-10";
            if (!string.IsNullOrEmpty(genre) && genre.ToLower() != "all")
            {
                url += $"&genres.name={Uri.EscapeDataString(genre.ToLower())}";
            }
            if (!string.IsNullOrEmpty(yearRange) && yearRange.ToLower() != "all")
            {
                var cleanYear = yearRange.Replace(" ", "");
                url += $"&year={cleanYear}";
            }
            if (!string.IsNullOrEmpty(sortBy) && sortBy.ToLower() != "none")
            {
                if (sortBy.ToLower() == "rating")
                {
                    url += "&sortField=rating.kp&sortType=-1";
                }
                else if (sortBy.ToLower() == "year")
                {
                    url += "&sortField=year&sortType=-1";
                }
            }
            else
            {
                url += "&sortField=votes.kp&sortType=-1";
            }
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("X-API-KEY", apiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) 
                return [];
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<SearchMovieResponseRaw>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (apiResponse == null || apiResponse.Docs == null) 
                return [];
            
            return apiResponse.Docs
                .Select(m => new MovieCard
                {
                    Id = m.Id,
                    Name = m.Name ?? "Без названия",
                    PosterUrl = m.Poster?.Url ?? "/images/default.jpg",
                    Rating = m.Rating?.Kp ?? m.Rating?.Imdb,
                    Year = m.Year,
                    Genres = m.Genres != null ? string.Join(", ", m.Genres.Select(g => g.Name)) : ""
                })
                .ToList();

        }

        private class SearchMovieResponseRaw
        {
            public List<MovieApi> Docs { get; set; } = [];
            public int Page {get; set;}
            public int Limit { get; set; }
            public int Total { get; set; }
            public int Pages { get; set; }
        }
    }