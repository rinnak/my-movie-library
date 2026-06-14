export interface MovieCard{
    id: number;
    name: string;
    posterUrl: string;
    rating: number | null;
    year: number | null;
}

export interface WatchedMovieDto{
    movieId: number;
    movieName: string;
    posterUrl: string;
    rating: number | null;
    year: number | null;
    userRating: number | null;
}

export interface UserLibraryStats{
    watchedCount: number;
    favoriteCount: number;
    averageRating: number;
}

export interface AuthResponse{
    token: string;
    userId: number;
}

export interface IMovieDetails {
    id: number;
    name: string;
    year: number;
    posterUrl: string;
    rating: number | null;
    genres: string[];
    description?: string;
}