import styles from './MovieCard.module.css';
import { useState, type MouseEvent } from "react";
import FavoritesIcon from '../../assets/favorites.svg?react';
import WatchedIcon from '../../assets/watched.svg?react';

interface MovieCardProps {
    id: number;
    name: string;
    posterUrl: string;
    rating: number | null;
    year: number | null;
    onCardClick: (id: number) => void;

    isFavorite?: boolean;
    isWatched?: boolean;
    onToggleFavorite?: () => void;
    onToggleWatched?: () => void;
}

export function MovieCard({id, name, posterUrl, rating, year, onCardClick, isFavorite = false, isWatched = false, onToggleFavorite,onToggleWatched}: MovieCardProps) {
    const [userRating, setUserRating] = useState<number | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const handlerFavoriteClick = (e: MouseEvent<HTMLButtonElement>) => {
        e.stopPropagation();
        const nextState = !isFavorite;
        onToggleFavorite?.();

        if(nextState && userRating == null) {
            setIsModalOpen(true);
        }
    };

    const handlerWatchedClick = (e: MouseEvent<HTMLButtonElement>) => {
        e.stopPropagation();
        const nextState = !isWatched;
        onToggleWatched?.();

        if(nextState && userRating == null) {
            setIsModalOpen(true);
        }
    };

    return (
        <>
            <div className={styles.card} onClick={() => onCardClick?.(id)}>
                <div className={styles.posterWrapper}>
                    <img src={posterUrl} alt={name} className={styles.poster} loading="lazy" />
                    {rating && (
                        <div className={styles.ratingBadge}>{rating.toFixed(1)}</div>
                    )}

                    {year && (
                        <div className={styles.yearBadge}>{year}</div>
                    )}
                    <div className={styles.title} title={name}>{name}</div>
                    <div className={styles.bottomGradient} ></div>
                    <div className={styles.actionsOverlay}>
                        <button
                            className={styles.actionBtn} // Оставляем кнопку просто круглой
                            onClick={(e) => handlerFavoriteClick(e)}
                            title="В избранное"
                        >
                            <FavoritesIcon
                                className={`${styles.favoriteItem} ${isFavorite ? styles.favoriteItemActive : ''}`}
                            />
                        </button>
                        <button
                            className={styles.actionBtn}
                            onClick={(e) => {handlerWatchedClick(e)}}
                            title="В просмотренные"
                        >
                            <WatchedIcon className={`${styles.watchedItem}  ${isWatched ? styles.watchedItemActive : ''}`}/>
                        </button>
                    </div>
                </div>

            </div>
        </>
    );
}