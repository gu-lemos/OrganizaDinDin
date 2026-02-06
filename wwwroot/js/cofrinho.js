(function() {
    'use strict';

    let saldo = 0;
    let elements = {};

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        saldo = window.cofrinhoSaldo || 0;
        cacheElements();
        attachEventListeners();
    }

    function cacheElements() {
        elements = {
            // Depósito
            depositoForm: document.getElementById('depositoForm'),
            depositoValor: document.getElementById('depositoValor'),
            depositoData: document.getElementById('depositoData'),
            depositoUsuario: document.getElementById('depositoUsuario'),
            btnConfirmarDeposito: document.getElementById('btnConfirmarDeposito'),
            depositoModal: document.getElementById('depositoModal'),
            // Resgate
            resgateForm: document.getElementById('resgateForm'),
            resgateValor: document.getElementById('resgateValor'),
            resgateData: document.getElementById('resgateData'),
            resgateUsuario: document.getElementById('resgateUsuario'),
            resgateMotivo: document.getElementById('resgateMotivo'),
            btnConfirmarResgate: document.getElementById('btnConfirmarResgate'),
            resgateModal: document.getElementById('resgateModal')
        };
    }

    function attachEventListeners() {
        elements.btnConfirmarDeposito.addEventListener('click', handleDepositar);
        elements.btnConfirmarResgate.addEventListener('click', handleResgatar);

        elements.depositoValor.addEventListener('input', handleValorInput);
        elements.resgateValor.addEventListener('input', handleValorInput);

        elements.depositoModal.addEventListener('show.bs.modal', () => resetForm('deposito'));
        elements.resgateModal.addEventListener('show.bs.modal', () => resetForm('resgate'));
    }

    function handleValorInput(e) {
        e.target.value = aplicarMascaraMoeda(e.target.value);
    }

    function resetForm(tipo) {
        if (tipo === 'deposito') {
            elements.depositoForm.reset();
            elements.depositoValor.value = '0,00';
            clearValidation(elements.depositoForm);
        } else {
            elements.resgateForm.reset();
            elements.resgateValor.value = '0,00';
            clearValidation(elements.resgateForm);
        }
    }

    function clearValidation(form) {
        form.querySelectorAll('.is-invalid, .is-valid').forEach(el => {
            el.classList.remove('is-invalid', 'is-valid');
        });
    }

    function validateDeposito() {
        let valid = true;

        const valor = converterParaCentavos(elements.depositoValor.value);
        if (valor <= 0) {
            elements.depositoValor.classList.add('is-invalid');
            elements.depositoValor.classList.remove('is-valid');
            valid = false;
        } else {
            elements.depositoValor.classList.remove('is-invalid');
            elements.depositoValor.classList.add('is-valid');
        }

        if (!elements.depositoData.value) {
            elements.depositoData.classList.add('is-invalid');
            elements.depositoData.classList.remove('is-valid');
            valid = false;
        } else {
            elements.depositoData.classList.remove('is-invalid');
            elements.depositoData.classList.add('is-valid');
        }

        if (!elements.depositoUsuario.value) {
            elements.depositoUsuario.classList.add('is-invalid');
            elements.depositoUsuario.classList.remove('is-valid');
            valid = false;
        } else {
            elements.depositoUsuario.classList.remove('is-invalid');
            elements.depositoUsuario.classList.add('is-valid');
        }

        return valid;
    }

    function validateResgate() {
        let valid = true;

        const valor = converterParaCentavos(elements.resgateValor.value);
        if (valor <= 0) {
            elements.resgateValor.classList.add('is-invalid');
            elements.resgateValor.classList.remove('is-valid');
            valid = false;
        } else {
            elements.resgateValor.classList.remove('is-invalid');
            elements.resgateValor.classList.add('is-valid');
        }

        if (!elements.resgateData.value) {
            elements.resgateData.classList.add('is-invalid');
            elements.resgateData.classList.remove('is-valid');
            valid = false;
        } else {
            elements.resgateData.classList.remove('is-invalid');
            elements.resgateData.classList.add('is-valid');
        }

        if (!elements.resgateUsuario.value) {
            elements.resgateUsuario.classList.add('is-invalid');
            elements.resgateUsuario.classList.remove('is-valid');
            valid = false;
        } else {
            elements.resgateUsuario.classList.remove('is-invalid');
            elements.resgateUsuario.classList.add('is-valid');
        }

        return valid;
    }

    async function handleDepositar() {
        if (!validateDeposito()) {
            showToast('Por favor, corrija os erros no formulário', 'error');
            return;
        }

        btnLoading(elements.btnConfirmarDeposito, 'Depositando...');

        const dto = {
            Valor: converterParaCentavos(elements.depositoValor.value),
            Data: elements.depositoData.value,
            UsuarioId: elements.depositoUsuario.value
        };

        try {
            const response = await fetch('/Cofrinho/Depositar', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(dto)
            });

            const result = await response.json();

            if (result.success) {
                showToast('Depósito realizado com sucesso!', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(result.message, 'error');
                btnReset(elements.btnConfirmarDeposito);
            }
        } catch (error) {
            showToast('Erro ao realizar depósito: ' + error, 'error');
            btnReset(elements.btnConfirmarDeposito);
        }
    }

    async function handleResgatar() {
        if (!validateResgate()) {
            showToast('Por favor, corrija os erros no formulário', 'error');
            return;
        }

        btnLoading(elements.btnConfirmarResgate, 'Resgatando...');

        const dto = {
            Valor: converterParaCentavos(elements.resgateValor.value),
            Data: elements.resgateData.value,
            UsuarioId: elements.resgateUsuario.value,
            Motivo: elements.resgateMotivo.value.trim() || null
        };

        try {
            const response = await fetch('/Cofrinho/Resgatar', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(dto)
            });

            const result = await response.json();

            if (result.success) {
                showToast('Resgate realizado com sucesso!', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(result.message, 'error');
                btnReset(elements.btnConfirmarResgate);
            }
        } catch (error) {
            showToast('Erro ao realizar resgate: ' + error, 'error');
            btnReset(elements.btnConfirmarResgate);
        }
    }

    function aplicarMascaraMoeda(valor) {
        valor = valor.replace(/\D/g, '');

        if (!valor || valor === '') {
            return '0,00';
        }

        const numero = parseInt(valor) || 0;
        valor = (numero / 100).toFixed(2);
        valor = valor.replace('.', ',');
        valor = valor.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');

        return valor;
    }

    function converterParaCentavos(valorFormatado) {
        const valorLimpo = valorFormatado.replace(/\./g, '').replace(',', '.');
        return Math.round(parseFloat(valorLimpo) * 100);
    }

})();
