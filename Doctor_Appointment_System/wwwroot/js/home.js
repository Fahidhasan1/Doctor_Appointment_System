// MOBILE NAV TOGGLE
const mobileMenuBtn = document.getElementById('mobileMenuBtn');
const navRight = document.getElementById('navRight');
const menuIcon = document.getElementById('menuIcon');

if (mobileMenuBtn && navRight && menuIcon) {
    mobileMenuBtn.addEventListener('click', function () {
        navRight.classList.toggle('open');
        menuIcon.textContent = navRight.classList.contains('open') ? '✕' : '☰';
    });

    document.addEventListener('click', function (event) {
        const isInsideNav = event.target.closest('.nav-container');
        if (!isInsideNav && navRight.classList.contains('open')) {
            navRight.classList.remove('open');
            menuIcon.textContent = '☰';
        }
    });
}

// SMOOTH SCROLL FOR IN-PAGE LINKS (about, contact, etc.)
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        const href = this.getAttribute('href');
        if (!href || href === '#') return;
        const target = document.querySelector(href);
        if (!target) return;

        e.preventDefault();
        target.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
    });
});

// AUTH MODAL LOGIC
const authOverlay = document.getElementById('authOverlay');
const loginModal = document.getElementById('loginModal');
const registerModal = document.getElementById('registerModal');

const navLoginBtn = document.getElementById('navLoginBtn');
const navRegisterBtn = document.getElementById('navRegisterBtn');
const heroBookBtn = document.getElementById('heroBookBtn');

const authCloseBtn1 = document.getElementById('authCloseBtn');
const authCloseBtn2 = document.getElementById('authCloseBtn2');

const authSwitchButtons = document.querySelectorAll('.auth-switch-btn');

function openAuthModal(mode) {
    if (!authOverlay || !loginModal || !registerModal) return;

    authOverlay.classList.remove('hidden');
    document.body.classList.add('auth-open');

    if (mode === 'register') {
        registerModal.classList.remove('hidden');
        loginModal.classList.add('hidden');
    } else {
        loginModal.classList.remove('hidden');
        registerModal.classList.add('hidden');
    }
}

function closeAuthModal() {
    if (!authOverlay) return;
    authOverlay.classList.add('hidden');
    document.body.classList.remove('auth-open');
}

if (navLoginBtn) {
    navLoginBtn.addEventListener('click', () => openAuthModal('login'));
}
if (navRegisterBtn) {
    navRegisterBtn.addEventListener('click', () => openAuthModal('register'));
}
if (heroBookBtn) {
    heroBookBtn.addEventListener('click', () => openAuthModal('login'));
}

if (authCloseBtn1) authCloseBtn1.addEventListener('click', closeAuthModal);
if (authCloseBtn2) authCloseBtn2.addEventListener('click', closeAuthModal);

// Close by clicking backdrop
if (authOverlay) {
    authOverlay.addEventListener('click', function (e) {
        if (e.target === authOverlay) {
            closeAuthModal();
        }
    });
}

// Close with ESC key
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        closeAuthModal();
    }
});

// Switch between login <-> register inside modal
authSwitchButtons.forEach(btn => {
    btn.addEventListener('click', function () {
        const target = this.getAttribute('data-target');
        if (target === 'register') {
            openAuthModal('register');
        } else {
            openAuthModal('login');
        }
    });
});

// AUTO-OPEN MODAL BASED ON QUERY STRING (?auth=login / ?auth=register)
const authFlagEl = document.getElementById('authOpenFlag');
if (authFlagEl) {
    const mode = authFlagEl.getAttribute('data-open-auth');
    if (mode === 'login' || mode === 'register') {
        openAuthModal(mode);
    }
}
