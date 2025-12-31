(function() {
    'use strict';

    let sidebar, iconMobile, overlay, btnToggle;

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        sidebar = document.getElementById('sidebar');
        iconMobile = document.getElementById('toggleIconMobile');
        overlay = document.getElementById('sidebarOverlay');
        btnToggle = document.getElementById('btnToggleSidebar');

        if (!sidebar || !btnToggle) return;

        btnToggle.addEventListener('click', toggleSidebar);

        if (overlay) {
            overlay.addEventListener('click', toggleSidebar);
        }
    }

    function toggleSidebar() {
        if (window.innerWidth >= 768) return;

        const isExpanded = sidebar.classList.toggle('expanded');

        if (isExpanded) {
            iconMobile.classList.remove('bi-list');
            iconMobile.classList.add('bi-x-lg');
            document.body.classList.add('sidebar-open');

            if (overlay) {
                overlay.classList.add('show');
            }
        } else {
            iconMobile.classList.remove('bi-x-lg');
            iconMobile.classList.add('bi-list');
            document.body.classList.remove('sidebar-open');

            if (overlay) {
                overlay.classList.remove('show');
            }
        }
    }

})();
