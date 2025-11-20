// Mobile menu toggle (sidebar)
const mobileMenuBtn = document.getElementById('mobileMenuBtn');
const navRight = document.getElementById('navRight');
const menuIcon = document.getElementById('menuIcon');

if (mobileMenuBtn && navRight && menuIcon) {
    mobileMenuBtn.addEventListener('click', function () {
        navRight.classList.toggle('open');
        menuIcon.textContent = navRight.classList.contains('open') ? '✕' : '☰';
    });

    // Close sidebar when clicking any nav or auth link
    const clickToCloseLinks = document.querySelectorAll('.nav-link, .auth-link');
    clickToCloseLinks.forEach(link => {
        link.addEventListener('click', function () {
            if (navRight.classList.contains('open')) {
                navRight.classList.remove('open');
                menuIcon.textContent = '☰';
            }
        });
    });

    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function (event) {
        const isInsideNav = event.target.closest('.nav-container');
        if (!isInsideNav && navRight.classList.contains('open')) {
            navRight.classList.remove('open');
            menuIcon.textContent = '☰';
        }
    });
}

// Smooth scroll for in-page links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        const href = this.getAttribute('href');
        if (!href || href === '#') return;

        const target = document.querySelector(href);
        if (target) {
            e.preventDefault();
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// About section cards (Mission / Vision / Values)
const highlightItems = document.querySelectorAll('.highlight-item');
highlightItems.forEach(item => {
    item.addEventListener('click', function () {
        highlightItems.forEach(i => i.classList.remove('active'));
        this.classList.add('active');
    });
});
