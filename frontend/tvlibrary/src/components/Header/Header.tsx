import {type FormEvent, useEffect, useState} from "react";
import styles from './Header.module.css';
import ProfileModal from "../ProfileModal/ProfileModal.tsx";
import SearchIcon from '../../assets/search.svg?react';
import api from '../../services/api';

interface HeaderProps {
    searchQuery: string;
    setSearchQuery: (searchQuery: string) => void;
}

export function Header({searchQuery, setSearchQuery}: HeaderProps) {
    const [isProfileOpen, setIsProfileOpen] = useState(false);

    const handleSearchSubmit = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        console.log(`Ищем фильм: ${searchQuery}`);
    };

    return(
        <>
            <header className={styles.header}>
                <form className={styles.searchForm} onSubmit={handleSearchSubmit}>
                    <div className={styles.searchWrapper}>
                        <SearchIcon className={styles.watchedItem} />
                        <input type="text" placeholder="Фильмы, сериалы, жанры"
                            className={styles.searchInput}
                               value={searchQuery}
                               onChange={(e) => setSearchQuery(e.target.value)}
                        />
                        {searchQuery && (
                            <button
                                type="button"
                                className={styles.clearBtn}
                                onClick={() => setSearchQuery('')}
                            >
                                &times;
                            </button>
                        )}
                    </div>
                </form>
                <div className={styles.profileSection}>
                    <button
                        className={styles.avatarBtn}
                        onClick={() => setIsProfileOpen(true)}
                        title="Настройки профиля"
                    >
                        <div className={styles.avatar}>E</div>
                    </button>
                </div>
            </header>
            <ProfileModal isOpen={isProfileOpen} onClose={() => setIsProfileOpen(false)} />
        </>
    )
}