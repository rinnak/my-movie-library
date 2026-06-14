    import styles from "./Sidebar.module.css"
    import HomeIcon from '../../assets/home.svg?react';
    import FavoritesIcon from '../../assets/favorites.svg?react';
    import WatchedIcon from '../../assets/watched.svg?react';
    import {useState} from "react";

    interface SidebarProps {
        activeTab: string;
        setActiveTab: (tab: string) => void;
        sortBy: 'none' | 'rating' | 'year';
        setSortBy: (sort:'none' | 'rating' | 'year') => void;
        yearRange: string;
        setYearRange: (range: string) => void;
        genreRange: string;
        setGenre: (genre: string) => void;
    }

    export function Sidebar({activeTab, setActiveTab, sortBy, setSortBy, yearRange, setYearRange}: SidebarProps){

        const [isOpen, setIsOpen] = useState(false);
        const navItems = [
            {id: 'main', label: 'Главная лента', icon: HomeIcon},
            {id: 'fav', label: 'Избранное', icon: FavoritesIcon},
            {id: 'watched', label: 'Просмотренное', icon: WatchedIcon}
        ];

        const yearsOptions = [
            {value: 'all', label: 'Все годы'},
            {value: '2020 - 2026', label: '2020 - 2026 (Новинки)'},
            {value: '2010 - 2019', label: '2010 - 2019'},
            {value: '2000 - 2009', label: '2000 - 2009'},
            {value: '1990 -1999', label: '90-е годы'}
        ];

        return(
            <>
                <button
                    className={`${styles.burgerToggle} ${isOpen ? styles.burgerToggleActive : ''}`}
                    onClick={() => setIsOpen(!isOpen)}
                    aria-label="Открыть меню"
                >
                    <span className={styles.burgerLine}></span>
                    <span className={styles.burgerLine}></span>
                    <span className={styles.burgerLine}></span>
                </button>
                <aside className={`${styles.sidebar} ${isOpen ? styles.sidebarOpen : ''}`}>
                    <div className={styles.logoBlock}>
                        <div className={styles.logoTitle}><span>My</span> Library</div>
                        <div className={styles.logoSub}>Фильмотека</div>
                    </div>

                    <nav>
                        <ul className={styles.menuList}>
                            {navItems.map(item => {
                                const Icon = item.icon;

                                return (
                                    <li key={item.id}>
                                        <button
                                            className={`${styles.menuItem} ${activeTab === item.id ? styles.menuItemActive : ''}`}
                                            onClick={() => {
                                                setActiveTab(item.id);
                                                setIsOpen(false);
                                            }}
                                        >

                                            <Icon className={styles.menuIcon} />
                                            {item.label}
                                        </button>
                                    </li>
                                );
                            })}
                        </ul>
                    </nav>
                    <div className = {styles.sectionTitle}>Сортировка</div>
                    <div className = {styles.filterGroup}>
                        <div className={styles.filterButtonGroup}>
                            <button className={`${styles.filterButton} ${sortBy==='none' ? styles.filterButtonActive : ''}`} onClick={() => setSortBy('none')}>Без сортировки</button>
                            <button
                                className={`${styles.filterButton} ${sortBy === 'rating' ? styles.filterButtonActive : ''}`}
                                onClick={() => setSortBy('rating')}
                            >По рейтингу</button>
                            <button
                                className={`${styles.filterButton} ${sortBy === 'year' ? styles.filterButtonActive : ''}`}
                                onClick={() => setSortBy('year')}
                            >По новизне</button>

                        </div>
                    </div>

                    <div className={styles.sectionTitle}>Период выпуска</div>
                    <div className={styles.filterGroup}>
                        <select className={styles.selectInput} value={yearRange} onChange={(e) => setYearRange(e.target.value)}>
                            {yearsOptions.map(item => (
                                <option key={item.value} value={item.value}>{item.label}</option>
                            ))}
                        </select>
                    </div>
                </aside>
            </>
        )
    }