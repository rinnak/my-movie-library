import { useParams, useNavigate } from "react-router-dom";
import {useEffect, useState} from "react";
import api from "../../services/api";
import styles from "./MovieDetails.module.css";
import RatingIcon from "../../assets/rating.svg?react";
import HeartIcon from "../../assets/favorites.svg?react";

interface IPerson{
    id: number;
    name: string;
    photo: string;
    profession: string;
}

interface IMovieDetails{
    id: number;
    name: string;
    year: number | null;
    posterUrl: string;
    rating: number | null;
    genres: string[];
    countries: string[];
    persons: IPerson[];
    description?: string;
}

interface IMovieLocalStorage{
    id: number;
    name: string;
    posterUrl: string;
    rating: number;
    year: number | null;
    genres?: string;
}

export function MovieDetailsPage(){
    const { id }  = useParams<{id: string}>();
    const navigate = useNavigate();

    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [movie, setMovie] = useState<IMovieDetails | null>(null);
    const [isFavorite, setIsFavorite] = useState<boolean>(false);
    const [isWatched, setIsWatched] = useState<boolean>(false);

    useEffect(() => {
        if (!id) return;

        const fetchData = async () => {
            let activeMovie: IMovieDetails | null = null;
            try{
                setLoading(true);
                setError(null);

                const res = await api.get<IMovieDetails>(`/movies/${id}`);
                activeMovie = res.data;

            }
            catch (err: any){
                console.error("Ошибка загрузки деталей фильма ", err);
                const mockMovies: IMovieDetails[] = [
                    {
                        id: 101,
                        name: "Интерстеллар",
                        year: 2014,
                        rating: 8.6,
                        countries: ["США", "Великобритания"],
                        genres: ["Фантастика", "Драма", "Приключения"],
                        description: "Когда засуха, пыльные бури и вымирание растений ставят человечество на грань исчезновения, группа исследователей отправляется в самое важное путешествие в истории: им предстоит узнать, есть ли у человечества будущее среди звезд.",
                        posterUrl: "https://avatars.mds.yandex.net/i?id=aee50eb150f39f5f2aa916ecdd0920522d627ec7-12797344-images-thumbs&n=13",
                        persons: [
                            { id: 1, name: "Мэттью Макконахи", photo: "https://upload.wikimedia.org/wikipedia/commons/4/46/Leonardo_Dicaprio_Cannes_2019.jpg", profession: "актер" },
                            { id: 2, name: "Энн Хэтэуэй", photo: "https://upload.wikimedia.org/wikipedia/commons/4/46/Leonardo_Dicaprio_Cannes_2019.jpg", profession: "актер" },
                            { id: 3, name: "Джессика Честейн", photo: "https://upload.wikimedia.org/wikipedia/commons/4/46/Leonardo_Dicaprio_Cannes_2019.jpg", profession: "актер" },
                            { id: 4, name: "Кристофер Нолан", photo: "", profession: "режиссер" },
                            { id: 5, name: "Ханс Циммер", photo: "", profession: "композитор" }
                        ]
                    },
                    {
                        id: 102,
                        name: "Начало",
                        year: 2010,
                        rating: 8.7,
                        countries: ["США", "Великобритания"],
                        genres: ["Фантастика", "Боевик", "Триллер"],
                        description: "Кобб — талантливый вор, лучший в опасном искусстве извлечения: он крадет ценные секреты из глубин подсознания во время сна, когда человеческий разум наиболее уязвим. Но теперь ему нужно совершить обратное — зародить идею.",
                        posterUrl: "http://images-s.kinorium.com/movie/poster/472809/w1500_2975883.jpg",
                        persons: [
                            { id: 11, name: "Леонардо Ди Каприо", photo: "https://upload.wikimedia.org/wikipedia/commons/4/46/Leonardo_Dicaprio_Cannes_2019.jpg", profession: "актер" },
                            { id: 12, name: "Том Харди", photo: "https://upload.wikimedia.org/wikipedia/commons/4/46/Leonardo_Dicaprio_Cannes_2019.jpg", profession: "актер" },
                            { id: 13, name: "Кристофер Нолан", photo: "", profession: "режиссер" }
                        ]
                    },
                    {
                        id: 103,
                        name: "Бегущий по лезвию 2049",
                        year: 2017,
                        rating: 7.8,
                        countries: ["США", "Великобритания", "Канада"],
                        genres: ["Фантастика", "Детектив", "Триллер"],
                        description: "В новый век репликанты стали безопасными и послушными рабами. Офицер К — новый блейд-раннер в департаменте полиции Лос-Анджелеса, чья задача — держать под контролем старых моделей, но он случайно натыкается на секрет прошлого.",
                        posterUrl: "https://ir.ozone.ru/s3/multimedia-1-w/w1200/6918184112.jpg",
                        persons: [
                            { id: 21, name: "Райан Гослинг", photo: "https://upload.wikimedia.org/wikipedia/commons/4/46/Leonardo_Dicaprio_Cannes_2019.jpg", profession: "актер" },
                            { id: 22, name: "Харрисон Форд", photo: "https://upload.wikimedia.org/wikipedia/commons/4/46/Leonardo_Dicaprio_Cannes_2019.jpg", profession: "актер" },
                            { id: 23, name: "Дени Вильнёв", photo: "", profession: "режиссер" }
                        ]
                    }
                ];

                activeMovie = mockMovies.find(m => m.id === Number(id)) || null;
            }
            finally{
                setLoading(false);
            }
            if (activeMovie) {
                setMovie(activeMovie);

                const favorite = localStorage.getItem("local_favorites");
                const watch = localStorage.getItem("local_watched");
                const favorites: IMovieLocalStorage[] = favorite ? JSON.parse(favorite) : [];
                const watched: IMovieLocalStorage[] = watch ? JSON.parse(watch) : [];

                setIsWatched(watched.some((item) => item.id === activeMovie!.id));
                setIsFavorite(favorites.some(m => m.id === activeMovie!.id));
            }
            else {
                setError("Не удалось загрузить информацию о фильме");
            }
        };
        fetchData();
    }, [id]);

    const toggleFavorite = () => {
        if (!movie) return;
        const saved = localStorage.getItem("local_favorites");
        let items: IMovieLocalStorage[] = saved ? JSON.parse(saved) : [];
        const exists = items.some(m => m.id === movie.id)

        if (exists) {
            items = items.filter(m => m.id !== movie.id);
            setIsFavorite(false);
        }
        else{
            items.push({
                id: movie.id,
                name: movie.name,
                posterUrl: movie.posterUrl,
                rating: movie.rating,
                year: movie.year,
                genres: movie.genres?.join(", ") || ""
            });
            setIsFavorite(true);
        }
        localStorage.setItem("local_favorites", JSON.stringify(items));
        window.dispatchEvent(new Event("storage"));
    }

    const toggleWatched = () => {
        if (!movie) return;
        const saved = localStorage.getItem("local_watched");
        let items: IMovieLocalStorage[] = saved ? JSON.parse(saved) : [];
        const exists = items.some(m => m.id === movie.id);
        if (exists) {
            items = items.filter(m => m.id !== movie.id);
            setIsWatched(false);
        }
        else{
            items.push({
                id: movie.id,
                name: movie.name,
                posterUrl: movie.posterUrl,
                rating: movie.rating,
                year: movie.year,
                genres: movie.genres?.join(", ") || ""
            });
            setIsWatched(true);
        }
        localStorage.setItem("local_watched", JSON.stringify(items));
        window.dispatchEvent(new Event("storage"));
    };

    if (loading) return <div className={styles.centeredState}>Загрузка информации...</div>;
    if (error || !movie) return <div className={styles.centeredState}>{error || "Фильм не найден"}</div>;

    const actors = movie.persons?.filter(p => p.profession.toLowerCase().includes("актер")) || [];
    const staff = movie.persons?.filter(p =>  !p.profession.toLowerCase().includes("актер")) || [];

    return (
        <div className={styles.pageContainer}>
            <button className={styles.backBtn} onClick={() => navigate(-1)}>&larr;  Назад</button>
            <div className={styles.mainGrid}>
                <div className={styles.leftColumn}>
                    <div className={styles.posterWrapper}>
                        <img src={movie.posterUrl} className={styles.poster} alt={movie.name} />
                        {movie.rating && (
                            <div className={styles.ratingBadge}>
                                <RatingIcon className={styles.ratingIcon}/>
                                <span>{movie.rating.toFixed(1)}</span>
                            </div>
                        )}
                    </div>

                    <div className={styles.actionButtons}>
                        <button
                            className={`${styles.favBtn} ${isFavorite ? styles.favBtnActive : ""}`}
                            onClick={toggleFavorite}
                        >
                           <HeartIcon className={styles.heartIcon} style={{fill: isFavorite ? "#ff4757" : "none"}}/>
                            {isFavorite ? "В избранном" : "В избранное"}
                        </button>
                        <button
                            className={`${styles.watchedBtn} ${isWatched ? styles.watchedBtnActive : ""}`}
                            onClick={toggleWatched}
                        >
                            {isWatched ? "Просмотрен" : "Отметить просмотренным"}
                        </button>
                    </div>
                </div>

                <div className={styles.centerColumn}>
                    <h1 className={styles.title}>{movie.name}</h1>
                    <span className={styles.subTitle}>О фильме</span>

                    <table className={styles.infoTable}>
                        <tbody>
                        {movie.year && (
                            <tr>
                                <td>Год производства</td>
                                <td>{movie.year}</td>
                            </tr>
                        )}
                        {movie.countries && movie.countries.length > 0 && (
                            <tr>
                                <td>Страна</td>
                                <td>{movie.countries.join(", ")}</td>
                            </tr>
                        )}
                        {movie.genres && movie.genres.length > 0 && (
                            <tr>
                                <td>Жанр</td>
                                <td className={styles.capitalize}>{movie.genres.join(", ")}</td>
                            </tr>
                        )}
                        {Array.from(new Set(staff.map(s => s.profession))).map(profession => {
                            const names = staff.filter(s => s.profession === profession).map(s => s.name).join(", ");
                            return (
                                <tr key={profession}>
                                    <td className = {styles.capitalize}>{profession}</td>
                                    <td>{names}</td>
                                </tr>
                            )
                        })}
                        </tbody>
                    </table>
                    {movie.description && (
                        <div className={styles.description}>
                            <h3>Описание</h3>
                            <p>{movie.description}</p>
                        </div>
                    )}
                </div>

                <div className={styles.rightColumn}>
                    <h3 className={styles.sectionTitle}>В главных ролях</h3>
                    {actors.length > 0 ? (
                        <ul className={styles.actorsList}>
                            {actors.slice(0, 12). map((actor) => (
                                <li key = {actor.id} className={styles.actorItem}>
                                    {actor.photo && (
                                        <img
                                            src = {actor.photo}
                                            alt = {actor.name}
                                            className={styles.actorPhoto}
                                        />
                                    )}
                                    <span className={styles.actorName}>{actor.name}</span>
                                </li>
                            ))}
                        </ul>
                    ) : (
                        <p className={styles.emptyText}>Нет информации об актерах</p>
                        )}
                </div>

            </div>
        </div>
    )
}