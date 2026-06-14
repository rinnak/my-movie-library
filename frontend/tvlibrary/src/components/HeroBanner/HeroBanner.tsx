import {useEffect, useState} from "react";
import api from '../../services/api'
import styles from './HeroBanner.module.css';
import type {IMovieDetails} from "../../../types";
import { useNavigate } from "react-router-dom";
import RatingIcon from '../../assets/rating.svg?react';
import HeartIcon from '../../assets/favorites.svg?react';

interface IMovieLocalStorage{
    id: number;
    name: string;
    posterUrl: string;
    rating: number | null;
    year: number | null;
    genres?: string;
}

export function HeroBanner() {
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [movies, setMovies] = useState<IMovieDetails[]>([]);
    const [currentSlide, setCurrentSlide] = useState<number>(0);
    const [favIds, setFavIds] = useState<number[]>(() => {
        const saved = localStorage.getItem('local_favorites');
        return saved ? JSON.parse(saved).map((m: IMovieLocalStorage) => m.id) : [];
    });

    const navigate = useNavigate();

    useEffect(() => {
        const fetchHerpMovies = async () => {
            try{
                setLoading(true);
                const res = await api.get<IMovieDetails[]>(`/movies/hero-list`);
                setMovies(res.data);
            }
            catch (err: any) {
                console.warn('Сервер недоступен, активирован фронтенд-MOCK для верстки:', err.message);
                const mockMovies: IMovieDetails[] = [
                    {
                        id: 101,
                        name: "Интерстеллар",
                        year: 2014,
                        rating: 8.6,
                        description: "Когда засуха, пыльные бури и вымирание растений ставят человечество на грань исчезновения, группа исследователей отправляется в самое важное путешествие в истории: им предстоит узнать, есть ли у человечества будущее среди звезд.",
                        posterUrl: "https://avatars.mds.yandex.net/i?id=aee50eb150f39f5f2aa916ecdd0920522d627ec7-12797344-images-thumbs&n=13",
                        genres: ["Фантастика", "Драма", "Приключения"]
                    },
                    {
                        id: 102,
                        name: "Начало",
                        year: 2010,
                        rating: 8.7,
                        description: "Кобб — талантливый вор, лучший в опасном искусстве извлечения: он крадет ценные секреты из глубин подсознания во время сна, когда человеческий разум наиболее уязвим. Но теперь ему нужно совершить обратное — зародить идею.",
                        posterUrl: "http://images-s.kinorium.com/movie/poster/472809/w1500_2975883.jpg",
                        genres: ["Фантастика", "Боевик", "Триллер"]
                    },
                    {
                        id: 103,
                        name: "Бегущий по лезвию 2049",
                        year: 2017,
                        rating: 7.8,
                        description: "В новый век репликанты стали безопасными и послушными рабами. Офицер К — новый блейд-раннер в департаменте полиции Лос-Анджелеса, чья задача — держать под контролем старых моделей, но он случайно натыкается на секрет прошлого.",
                        posterUrl: "https://ir.ozone.ru/s3/multimedia-1-w/w1200/6918184112.jpg",
                        genres: ["Фантастика", "Детектив", "Триллер"]
                    }
                    ];
                setMovies(mockMovies);}
            finally {
                setLoading(false);
            }
        };
        fetchHerpMovies();
    }, []);

    useEffect(() => {
        const handleStorageChange = () => {
            const saved = localStorage.getItem('local_favorites');
            setFavIds(saved ? JSON.parse(saved).map((m: IMovieLocalStorage) => m.id) : []);
        };
        window.addEventListener('storage', handleStorageChange);
        return () => window.removeEventListener('storage', handleStorageChange);
    }, []);

    useEffect(() => {
        if (movies.length <= 1) return;
        const interval = setInterval(() => {
            setCurrentSlide((prev) => (prev + 1) % movies.length);
        }, 7000);
        return () => clearInterval(interval);
    }, [movies]);

    if (loading) return <div className={styles.bannerSkeleton}>Подбираем новинки...</div>
    if (error || movies.length === 0) return <div className = {styles.bannerError}>Не удалось загрузить новинки</div>

    const activeMovie = movies[currentSlide];
    const Icon = RatingIcon;

    const isCurrentFavorite = favIds.includes(activeMovie.id);

    const toggleFavorite = () => {
        const saved = localStorage.getItem('local_favorites');
        let items: IMovieLocalStorage[] = saved ? JSON.parse(saved) : [];
        const exists = items.some(m => m.id === activeMovie.id);

        if (exists) {
            items = items.filter(m => m.id !== activeMovie.id);
        }
        else{
            const movieToSave: IMovieLocalStorage = {
                id: activeMovie.id,
                name: activeMovie.name,
                posterUrl: activeMovie.posterUrl,
                rating: activeMovie.rating,
                year: activeMovie.year,
                genres: activeMovie.genres && activeMovie.genres.length > 0
                    ? activeMovie.genres.join(', ')
                    : ""
            };
            items.push(movieToSave);
        }
        localStorage.setItem('local_favorites', JSON.stringify(items));
        setFavIds(items.map(m => m.id));
        window.dispatchEvent(new Event('storage'));
    }

    return (
        <section className={styles.banner} style={{backgroundImage : `url(${activeMovie.posterUrl}`}}>
            <div className={styles.overlay}></div>
            <div className={styles.content}>
                <span className={styles.badge}>Выбор дня</span>
                <h1 className={styles.title}>{activeMovie.name}</h1>

                <div className={styles.meta}>
                    {activeMovie.rating && (
                        <div className={styles.ratingGroup}>
                            <Icon className={styles.menuIcon} />
                            <span className={styles.rating}>{activeMovie.rating.toFixed(1)}</span>
                        </div>
                    )}
                    <span className={styles.year}>{activeMovie.year}</span>
                    {activeMovie.genres && activeMovie.genres.length > 0 && (
                        <span className={styles.genres}>{activeMovie.genres.join(', ')}</span>
                    )}
                </div>
                <p className={styles.description}>{activeMovie.description || 'Описание этого фильма появится совсем скоро.'}</p>

                <div className={styles.actions}>
                    <button className={styles.btnPrimary} onClick={() => navigate(`/movie/${activeMovie.id}`)}>Подробнее</button>
                    <button className={styles.btnSecondary} onClick={toggleFavorite}>
                        <HeartIcon className={styles.btnIcon} style={{ fill: isCurrentFavorite ? '#ff4757' : 'none', stroke: isCurrentFavorite ? '#ff4757' : 'currentColor' }}/>
                        {isCurrentFavorite ? 'В избранном' : 'В избранное'}
                    </button>
                </div>
            </div>
            <div className={styles.pagination}>
                {movies.map((_, index) => (
                    <button
                        key={index}
                        className={`${styles.dot} ${index === currentSlide ? styles.dotActive : ""}`}
                        onClick={() => setCurrentSlide(index)}
                        aria-label={`Перейти к слайду ${index + 1}`}
                    ></button>
                ))}
            </div>
        </section>
    )
}