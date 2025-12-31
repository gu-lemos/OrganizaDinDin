(function() {
    'use strict';

    let formElements = {};

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        formElements.form = document.getElementById('registerForm');
        formElements.nome = document.getElementById('registerNome');
        formElements.email = document.getElementById('registerEmail');
        formElements.senha = document.getElementById('registerSenha');

        if (!formElements.form || !formElements.nome || !formElements.email || !formElements.senha) return;

        attachEventListeners();
    }

    function attachEventListeners() {
        formElements.nome.addEventListener('blur', () => validateField('nome'));
        formElements.nome.addEventListener('input', () => {
            if (formElements.nome.classList.contains('is-invalid') || formElements.nome.classList.contains('is-valid')) {
                validateField('nome');
            }
        });

        formElements.email.addEventListener('blur', () => validateField('email'));
        formElements.email.addEventListener('input', () => {
            if (formElements.email.classList.contains('is-invalid') || formElements.email.classList.contains('is-valid')) {
                validateField('email');
            }
        });

        formElements.senha.addEventListener('blur', () => validateField('senha'));
        formElements.senha.addEventListener('input', () => {
            if (formElements.senha.classList.contains('is-invalid') || formElements.senha.classList.contains('is-valid')) {
                validateField('senha');
            }
        });

        formElements.form.addEventListener('submit', handleFormSubmit);
    }

    function validateField(fieldName) {
        const field = formElements[fieldName];
        let isValid = true;

        if (fieldName === 'nome') {
            const nomeValue = field.value.trim();
            isValid = nomeValue !== '';
        } else if (fieldName === 'email') {
            const emailValue = field.value.trim();
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            isValid = emailValue !== '' && emailRegex.test(emailValue);
        } else if (fieldName === 'senha') {
            const senhaValue = field.value.trim();
            isValid = senhaValue.length >= 6;
        }

        if (!isValid) {
            field.classList.add('is-invalid');
            field.classList.remove('is-valid');
        } else {
            field.classList.remove('is-invalid');
            field.classList.add('is-valid');
        }

        return isValid;
    }

    function handleFormSubmit(e) {
        const nomeValid = validateField('nome');
        const emailValid = validateField('email');
        const senhaValid = validateField('senha');

        if (!nomeValid || !emailValid || !senhaValid) {
            e.preventDefault();
        }
    }

})();
