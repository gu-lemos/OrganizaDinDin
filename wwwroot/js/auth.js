(function () {
    'use strict';

    const regras = {
        obrigatorio: valor => valor.trim() !== '',
        email: valor => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(valor.trim()),
        senha: valor => valor.trim().length >= 6
    };

    document.querySelectorAll('.auth-form').forEach(form => {
        const campos = Array.from(form.querySelectorAll('[data-validar]'));

        const validar = campo => {
            const valido = regras[campo.dataset.validar](campo.value);
            campo.classList.toggle('is-invalid', !valido);
            campo.classList.toggle('is-valid', valido);
            return valido;
        };

        campos.forEach(campo => {
            campo.addEventListener('blur', () => validar(campo));
            campo.addEventListener('input', () => {
                if (campo.classList.contains('is-invalid') || campo.classList.contains('is-valid')) validar(campo);
            });
        });

        form.addEventListener('submit', e => {
            const todosValidos = campos.map(validar).every(Boolean);
            if (!todosValidos) {
                e.preventDefault();
                return;
            }

            const botao = form.querySelector('button[type="submit"]');
            botao.disabled = true;
            botao.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>${botao.dataset.carregando || 'Enviando...'}`;
        });
    });
})();
