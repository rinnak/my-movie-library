import {useEffect, type MouseEvent} from "react";
import styles from './ProfileModal.module.css';

interface ProfileModalProps{
    isOpen: boolean;
    onClose: () => void;
}

function ProfileModal({isOpen, onClose}: ProfileModalProps) {
    useEffect(() => {
        if(!isOpen) return;
        const handleKeyDown = (e: KeyboardEvent) => {
            if(e.key === 'Escape') onClose();
        }
        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [isOpen, onClose]);

    if (!isOpen) return null;
    const handleOverlayClick = (e: MouseEvent<HTMLDivElement>) => {
        if (e.target === e.currentTarget) onClose();
    };

    const handleLogOut = () => {
        console.log('выход из аккаунта');
        onClose();
    };

    return (
        <div className={styles.overlay} onClick={handleOverlayClick}>
            <div className={styles.modalContent}>
                <button className={styles.closeBtn} onClick={onClose} title="закрыть">&times;</button>

                <div className={styles.userHeader}>
                    <div className={styles.largeAvatar}>E</div>
                    <div className={styles.userEmail}>example@mail.ru</div>
                </div>
                <hr className={styles.divider} />
                <button className={styles.logoutBtn} onClick={handleLogOut}>
                    Выйти из аккаунта
                </button>
            </div>
        </div>
    )
}

export default ProfileModal