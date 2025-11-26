// Sidebar toggle (on mobile/tablet)
const sidebarToggleBtn = document.getElementById('sidebarToggleBtn');
const sidebarElement = document.getElementById('sidebar');

if (sidebarToggleBtn && sidebarElement) {
    sidebarToggleBtn.addEventListener('click', () => {
        sidebarElement.classList.toggle('sidebar-open');
    });

    document.addEventListener('click', function (e) {
        const isInsideSidebar = e.target.closest('#sidebar');
        const isToggle = e.target.closest('#sidebarToggleBtn');
        if (!isInsideSidebar && !isToggle && sidebarElement.classList.contains('sidebar-open')) {
            sidebarElement.classList.remove('sidebar-open');
        }
    });
}

// Manage Users submenu
const manageUsersToggle = document.getElementById('manageUsersToggle');
const manageUsersSubmenu = document.getElementById('manageUsersSubmenu');

if (manageUsersToggle && manageUsersSubmenu) {
    manageUsersToggle.addEventListener('click', () => {
        manageUsersSubmenu.classList.toggle('sidebar-submenu-show');
        manageUsersToggle.classList.toggle('submenu-open');
    });
}

// Sidebar date text
const todaySidebarText = document.getElementById('todaySidebarText');
if (todaySidebarText) {
    const today = new Date();
    const options = { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' };
    todaySidebarText.textContent = today.toLocaleDateString('en-GB', options);
}
