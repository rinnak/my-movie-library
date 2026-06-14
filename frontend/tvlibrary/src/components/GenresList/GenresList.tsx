import styles from './GenresList.module.css';

interface genresProps {
    activeGenre: string;
    setActiveGenre: (genre: string) => void;
}

export function GenresList({activeGenre, setActiveGenre}: genresProps) {
    const genres = [
        { value: 'all', label: 'Все жанры' },
        { value: 'боевик', label: 'Боевики' },
        { value: 'комедия', label: 'Комедии' },
        { value: 'драма', label: 'Драмы' },
        { value: 'фантастика', label: 'Фантастика' },
        { value: 'триллер', label: 'Триллеры' },
        { value: 'ужасы', label: 'Ужасы' },
        { value: 'мелодрама', label: 'Мелодрамы' },
        { value: 'мультфильм', label: 'Мультфильмы' }
    ];

    return (
        <div className={styles.genresContainer}>
            <div className={styles.genresTrack}>
                {genres.map(genre => (
                    <button
                        key={genre.value}
                        className={`${styles.genreButton} ${activeGenre === genre.value ? styles.genreButtonActive : ''}`}
                        onClick={() => setActiveGenre(genre.value)}
                    >
                        {genre.label}
                    </button>
                ))}
            </div>
        </div>
    )
}