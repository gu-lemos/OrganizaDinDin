(function() {
    'use strict';

    window.showToast = function(message, type = 'success') {
        const toastElement = document.getElementById('toastNotification');
        const toastMessage = document.getElementById('toastMessage');

        if (!toastElement || !toastMessage) return;

        toastMessage.textContent = message;

        toastElement.classList.remove('text-bg-success', 'text-bg-danger', 'text-bg-warning', 'text-bg-info', 'text-bg-primary');

        const typeMap = {
            'success': 'text-bg-success',
            'error': 'text-bg-danger',
            'warning': 'text-bg-warning',
            'info': 'text-bg-info'
        };

        toastElement.classList.add(typeMap[type] || 'text-bg-primary');

        const toast = new bootstrap.Toast(toastElement, {
            animation: true,
            autohide: true,
            delay: 4000
        });
        toast.show();
    };

})();
