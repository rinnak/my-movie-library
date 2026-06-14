import { useEffect, useState } from "react";
import styles from './MovieGrid.module.css';
import api from '../../services/api';
import { useNavigate } from "react-router-dom";
import { MovieCard } from "../MovieCard/MovieCard.tsx";
import { GenresList } from "../GenresList/GenresList.tsx";

interface IMovie {
    id: number;
    name: string;
    posterUrl: string;
    rating: number | null;
    year: number | null;
    genres?: string;
}

interface MovieGridProps {
    activeTab: string;
    sortBy: 'none' | 'rating' | 'year';
    yearRange: string;
    searchQuery: string;
}

export function MovieGrid({ activeTab, sortBy, yearRange, searchQuery }: MovieGridProps) {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [movies, setMovies] = useState<IMovie[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [activeGenre, setActiveGenre] = useState('all');
    const [debouncedSearchQuery, setDebouncedSearchQuery] = useState(searchQuery);

    const [favIds, setFavIds] = useState<number[]>(() => {
        const saved = localStorage.getItem('local_favorites');
        return saved ? JSON.parse(saved).map((m: IMovie) => m.id) : [];
    });

    const [watchedIds, setWatchedIds] = useState<number[]>(() => {
        const saved = localStorage.getItem('local_watched');
        return saved ? JSON.parse(saved).map((m: IMovie) => m.id) : [];
    });

    const handleGenreChange = (genre: string) => {
        setActiveGenre(genre);
        setCurrentPage(1);
    };

    const getPageNumbers = () => {
        const pages = [];
        const startPage = Math.max(1, currentPage - 2);
        for (let i = startPage; i <= startPage + 4; i++) {
            pages.push(i);
        }
        return pages;
    };

    useEffect(() => { console.log("activeTab хук сброса"); setCurrentPage(1); }, [activeTab]);
    useEffect(() => { console.log("sortBy хук сброса"); setCurrentPage(1); }, [sortBy]);
    useEffect(() => { console.log("yearRange хук сброса"); setCurrentPage(1); }, [yearRange]);

    useEffect(() => {
        console.log(" хук поиска searchQuery");
        const timer = setTimeout(() => {
            setCurrentPage(1);
            setDebouncedSearchQuery(searchQuery);
        }, 600);
        return () => clearTimeout(timer);
    }, [searchQuery]);

    useEffect(() => {
        console.log(" Хук fetchMovies", {
            currentPage,
            activeGenre,
            activeTab,
            sortBy,
            yearRange,
            debouncedSearchQuery
        });
        const fetchMovies = async () => {
            try {
                setLoading(true);

                if (activeTab === 'fav' || activeTab === 'watched') {
                    const storageKey = activeTab === 'fav' ? 'local_favorites' : 'local_watched';
                    const saved = localStorage.getItem(storageKey);
                    let localMovies: IMovie[] = saved ? JSON.parse(saved) : [];

                    console.log(localMovies);
                    if (debouncedSearchQuery.trim()) {
                        localMovies = localMovies.filter(movie =>
                            movie.name.toLowerCase().includes(debouncedSearchQuery.toLowerCase())
                        );
                    }

                    if (activeGenre && activeGenre !== 'all'){
                        localMovies = localMovies.filter(movie => {
                            if (!movie.genres) return false;
                            const genresList = movie.genres.split(',').map(g => g.trim().toLowerCase());
                            return genresList.includes(activeGenre.toLowerCase());
                        })
                    }
                    if(yearRange && yearRange !== 'all'){
                        const parts = yearRange.split('-');
                        if(parts.length === 2 && parseInt(parts[0]) && parseInt(parts[1])){
                            const startYear = parseInt(parts[0]);
                            const endYear = parseInt(parts[1]);
                            localMovies = localMovies.filter(movie =>
                                movie.year !== null && movie.year >= startYear && movie.year <= endYear
                            );
                        }
                    }

                    if (sortBy === 'rating'){
                        localMovies.sort((a, b) => (b.rating ?? 0) - (a.rating ?? 0));
                    }
                    else if (sortBy === 'year'){
                        localMovies.sort((a, b) => (b.year ?? 0) - (a.year ?? 0));
                    }
                    const limit = 14;
                    const startIndex = (currentPage - 1) * limit;
                    const endIndex = startIndex + limit;

                    setMovies(localMovies.slice(startIndex, endIndex));
                    return;
                }

                if (debouncedSearchQuery.trim() !== '') {
                    const res = await api.get(`/movies/search?query=${encodeURIComponent(debouncedSearchQuery)}`);
                    setMovies(res.data.docs || []);
                    return;
                }

                const res = await api.get(
                    `/recommendation/feed?page=${currentPage}&limit=14&genre=${activeGenre}&sortBy=${sortBy}&yearRange=${yearRange}&favIds=${favIds.join(',')}&watchedIds=${watchedIds.join(',')}`
                );
                setMovies(res.data);
            } catch (e: any) {
                console.warn('Используем макетные данные:', e.message);
                const mockMovies: IMovie[] = [
                    { id: 1, name: "Довод", posterUrl: "https://ir.ozone.ru/s3/multimedia-1-w/w1200/6918184112.jpg", rating: 9.0, year: 2020 }
                ];
                setMovies(mockMovies);
            } finally {
                setLoading(false);
            }
        };

        fetchMovies();
    }, [currentPage, activeGenre, activeTab, sortBy, yearRange, debouncedSearchQuery]);

    const toggleLocalList = (key: 'local_favorites' | 'local_watched', movie: IMovie) => {
        const saved = localStorage.getItem(key);
        let items: IMovie[] = saved ? JSON.parse(saved) : [];
        const exists = items.some(m => m.id === movie.id);

        if (exists) {
            items = items.filter(m => m.id !== movie.id);
        } else {
            items.push(movie);
        }
        localStorage.setItem(key, JSON.stringify(items));

        const updatedIds = items.map(m => m.id);

        if (key === 'local_favorites') {
            setFavIds(updatedIds);
            if (activeTab === 'fav') {
                setMovies(prev => prev.filter(m => m.id !== movie.id));
            }
        } else {
            setWatchedIds(updatedIds);
            if (activeTab === 'watched') {
                setMovies(prev => prev.filter(m => m.id !== movie.id));
            }
        }
    };

    return (
        <section className={styles.gridSection}>
            <GenresList activeGenre={activeGenre} setActiveGenre={handleGenreChange} />

            <div className={styles.gridHeader}>
                <h2>
                    {searchQuery ? `Результаты поиска по запросу "${searchQuery}"` : activeTab === 'fav' ? `Избранные` : activeTab === 'watched' ? `Просмотренные` : 'Рекомендации для вас'}
                    <span className={styles.totalMovies}> ({movies.length})</span>
                </h2>

                <div className={styles.pagination}>
                    <button className={styles.pageBtn} disabled={currentPage === 1} onClick={() => setCurrentPage(p => p - 1)}>&lt;</button>
                    {getPageNumbers().map(page => (
                        <button
                            key={page}
                            className={`${styles.pageNumber} ${currentPage === page ? styles.pageActive : ''}`}
                            onClick={() => setCurrentPage(page)}
                        >{page}</button>
                    ))}
                    <button className={styles.pageBtn} disabled={(movies.length)%14 != 0} onClick={() => setCurrentPage(p => p + 1)}>&gt;</button>
                </div>
            </div>

            {loading ? (
                <div className={styles.skeletonGrid}>
                    {[...Array(14)].map((_, i) => <div key={i} className={styles.skeletonCard} />)}
                </div>
            ) : (
                <div className={styles.grid}>
                    {movies.map((movie, index) => (
                        <MovieCard
                            key={`${movie.id}-${index}`}
                            id={movie.id}
                            name={movie.name}
                            posterUrl={movie.posterUrl}
                            rating={movie.rating}
                            year={movie.year}
                            onCardClick={(id) => navigate(`/movie/${id}`)}
                            isFavorite={favIds.includes(movie.id)}
                            isWatched={watchedIds.includes(movie.id)}
                            onToggleFavorite={() => toggleLocalList('local_favorites', movie)}
                            onToggleWatched={() => toggleLocalList('local_watched', movie)}
                        />
                    ))}
                </div>
            )}
            <div className={styles.mobilePagination}>
                <button
                    className={`${styles.mobileBtn} ${styles.pageBtn}`}
                    disabled={currentPage === 1}
                    onClick={() => {setCurrentPage( p => p - 1 );
                    window.scrollTo({top: 0, behavior: 'smooth'}); }}
                >
                    &lt; Назад
                </button>
                <span className={styles.mobilePageInfo}>Страница {currentPage}</span>
                <button
                    className={`${styles.mobileBtn} ${styles.pageBtn}`}
                    disabled={movies.length %14 !== 0}
                    onClick={() => {setCurrentPage( p => p + 1 );
                    window.scrollTo({ top: 0, behavior: 'smooth' });}}
                >&gt; Вперед</button>
            </div>
        </section>
    );
}