window.btnLoading = function(button, text) {
    button._originalHTML = button.innerHTML;
    button.disabled = true;
    button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>' + (text || 'Carregando...');
};

window.btnReset = function(button) {
    button.disabled = false;
    button.innerHTML = button._originalHTML;
};
