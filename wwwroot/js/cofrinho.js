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
            // Filtros
            filterCard: document.getElementById('filterCard'),
            filterCardHeader: document.getElementById('filterCardHeader'),
            filterCardBody: document.getElementById('filterCardBody'),
            btnLimparFiltros: document.getElementById('btnLimparFiltros'),
            dropdownTipo: document.querySelector('.dropdown-tipo'),
            dropdownTipoToggle: document.querySelector('.dropdown-tipo-toggle'),
            dropdownTipoText: document.querySelector('.dropdown-tipo-text'),
            dropdownTipoCheckboxes: document.querySelectorAll('.dropdown-tipo-item input[type="checkbox"]'),
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
            resgateModal: document.getElementById('resgateModal'),
            // Editar
            editarForm: document.getElementById('editarForm'),
            editarId: document.getElementById('editarId'),
            editarTipo: document.getElementById('editarTipo'),
            editarValor: document.getElementById('editarValor'),
            editarData: document.getElementById('editarData'),
            editarUsuario: document.getElementById('editarUsuario'),
            editarMotivo: document.getElementById('editarMotivo'),
            editarMotivoGroup: document.getElementById('editarMotivoGroup'),
            btnConfirmarEditar: document.getElementById('btnConfirmarEditar'),
            editarModal: document.getElementById('editarModal'),
            // Excluir
            excluirId: document.getElementById('excluirId'),
            excluirDescricao: document.getElementById('excluirDescricao'),
            btnConfirmarExcluir: document.getElementById('btnConfirmarExcluir'),
            excluirModal: document.getElementById('excluirModal'),
            // Meta
            metaForm: document.getElementById('metaForm'),
            metaValor: document.getElementById('metaValor'),
            btnConfirmarMeta: document.getElementById('btnConfirmarMeta'),
            metaModal: document.getElementById('metaModal')
        };
    }

    function attachEventListeners() {
        elements.btnConfirmarDeposito.addEventListener('click', handleDepositar);
        elements.btnConfirmarResgate.addEventListener('click', handleResgatar);
        elements.btnConfirmarEditar.addEventListener('click', handleEditar);
        elements.btnConfirmarExcluir.addEventListener('click', handleExcluir);
        elements.btnConfirmarMeta.addEventListener('click', handleSetMeta);

        if (elements.filterCardHeader) {
            elements.filterCardHeader.addEventListener('click', toggleFilterCard);
        }
        if (elements.btnLimparFiltros) {
            elements.btnLimparFiltros.addEventListener('click', () => { window.location.href = '/Cofrinho/Index'; });
        }
        if (elements.dropdownTipo) {
            setupDropdownTipo();
        }

        elements.depositoValor.addEventListener('input', handleValorInput);
        elements.resgateValor.addEventListener('input', handleValorInput);
        elements.editarValor.addEventListener('input', handleValorInput);
        elements.editarTipo.addEventListener('change', () => {
            toggleMotivoGroup(parseInt(elements.editarTipo.value));
        });
        elements.metaValor.addEventListener('input', handleValorInput);

        elements.depositoModal.addEventListener('show.bs.modal', () => resetForm('deposito'));
        elements.resgateModal.addEventListener('show.bs.modal', () => resetForm('resgate'));
        elements.metaModal.addEventListener('show.bs.modal', () => resetFormMeta());

        // Delegação para botões de editar/excluir na tabela
        document.addEventListener('click', function(e) {
            const editBtn = e.target.closest('.btn-editar');
            if (editBtn) {
                abrirEditar(editBtn);
                return;
            }
            const deleteBtn = e.target.closest('.btn-excluir');
            if (deleteBtn) {
                abrirExcluir(deleteBtn);
            }
        });
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

    function resetFormMeta() {
        clearValidation(elements.metaForm);
        // mantém o valor existente se houver
    }

    function clearValidation(form) {
        form.querySelectorAll('.is-invalid, .is-valid').forEach(el => {
            el.classList.remove('is-invalid', 'is-valid');
        });
    }

    // ---- Depósito ----

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

    // ---- Resgate ----

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

    // ---- Editar ----

    function abrirEditar(btn) {
        const id = btn.dataset.id;
        const valor = parseInt(btn.dataset.valor);
        const data = btn.dataset.data;
        const usuario = btn.dataset.usuario;
        const tipo = parseInt(btn.dataset.tipo);
        const motivo = btn.dataset.motivo || '';

        elements.editarId.value = id;
        elements.editarTipo.value = tipo;
        elements.editarValor.value = formatarCentavos(valor);
        elements.editarData.value = data;
        elements.editarUsuario.value = usuario;
        elements.editarMotivo.value = motivo;

        toggleMotivoGroup(tipo);

        clearValidation(elements.editarForm);

        const modal = bootstrap.Modal.getOrCreateInstance(elements.editarModal);
        modal.show();
    }

    function validateEditar() {
        let valid = true;

        const valor = converterParaCentavos(elements.editarValor.value);
        if (valor <= 0) {
            elements.editarValor.classList.add('is-invalid');
            elements.editarValor.classList.remove('is-valid');
            valid = false;
        } else {
            elements.editarValor.classList.remove('is-invalid');
            elements.editarValor.classList.add('is-valid');
        }

        if (!elements.editarData.value) {
            elements.editarData.classList.add('is-invalid');
            elements.editarData.classList.remove('is-valid');
            valid = false;
        } else {
            elements.editarData.classList.remove('is-invalid');
            elements.editarData.classList.add('is-valid');
        }

        if (!elements.editarUsuario.value) {
            elements.editarUsuario.classList.add('is-invalid');
            elements.editarUsuario.classList.remove('is-valid');
            valid = false;
        } else {
            elements.editarUsuario.classList.remove('is-invalid');
            elements.editarUsuario.classList.add('is-valid');
        }

        return valid;
    }

    async function handleEditar() {
        if (!validateEditar()) {
            showToast('Por favor, corrija os erros no formulário', 'error');
            return;
        }

        btnLoading(elements.btnConfirmarEditar, 'Salvando...');

        const dto = {
            Id: elements.editarId.value,
            Valor: converterParaCentavos(elements.editarValor.value),
            Data: elements.editarData.value,
            UsuarioId: elements.editarUsuario.value,
            Tipo: parseInt(elements.editarTipo.value),
            Motivo: elements.editarMotivo.value.trim() || null
        };

        try {
            const response = await fetch('/Cofrinho/Editar', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(dto)
            });

            const result = await response.json();

            if (result.success) {
                showToast('Transação atualizada com sucesso!', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(result.message, 'error');
                btnReset(elements.btnConfirmarEditar);
            }
        } catch (error) {
            showToast('Erro ao editar transação: ' + error, 'error');
            btnReset(elements.btnConfirmarEditar);
        }
    }

    function toggleMotivoGroup(tipo) {
        // 0 = Deposito, 1 = Resgate
        elements.editarMotivoGroup.style.display = tipo === 0 ? 'none' : 'block';
    }

    // ---- Excluir ----

    function abrirExcluir(btn) {
        elements.excluirId.value = btn.dataset.id;
        elements.excluirDescricao.textContent = btn.dataset.descricao;

        const modal = bootstrap.Modal.getOrCreateInstance(elements.excluirModal);
        modal.show();
    }

    async function handleExcluir() {
        const id = elements.excluirId.value;
        if (!id) return;

        btnLoading(elements.btnConfirmarExcluir, 'Excluindo...');

        try {
            const response = await fetch('/Cofrinho/Excluir', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Id: id })
            });

            const result = await response.json();

            if (result.success) {
                showToast('Transação excluída com sucesso!', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(result.message, 'error');
                btnReset(elements.btnConfirmarExcluir);
            }
        } catch (error) {
            showToast('Erro ao excluir transação: ' + error, 'error');
            btnReset(elements.btnConfirmarExcluir);
        }
    }

    // ---- Meta ----

    function validateMeta() {
        const valor = converterParaCentavos(elements.metaValor.value);
        if (valor <= 0) {
            elements.metaValor.classList.add('is-invalid');
            elements.metaValor.classList.remove('is-valid');
            return false;
        }
        elements.metaValor.classList.remove('is-invalid');
        elements.metaValor.classList.add('is-valid');
        return true;
    }

    async function handleSetMeta() {
        if (!validateMeta()) {
            showToast('Por favor, informe um valor válido para a meta', 'error');
            return;
        }

        btnLoading(elements.btnConfirmarMeta, 'Salvando...');

        const dto = {
            Valor: converterParaCentavos(elements.metaValor.value)
        };

        try {
            const response = await fetch('/Cofrinho/SetMeta', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(dto)
            });

            const result = await response.json();

            if (result.success) {
                showToast('Meta definida com sucesso!', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(result.message, 'error');
                btnReset(elements.btnConfirmarMeta);
            }
        } catch (error) {
            showToast('Erro ao definir meta: ' + error, 'error');
            btnReset(elements.btnConfirmarMeta);
        }
    }

    // ---- Filtros ----

    function toggleFilterCard() {
        elements.filterCard.classList.toggle('collapsed');
        elements.filterCardHeader.classList.toggle('collapsed');
        elements.filterCardBody.classList.toggle('collapsed');
    }

    function setupDropdownTipo() {
        elements.dropdownTipoToggle.addEventListener('click', (e) => {
            e.preventDefault();
            elements.dropdownTipo.classList.toggle('active');
        });

        document.addEventListener('click', (e) => {
            if (!elements.dropdownTipo.contains(e.target)) {
                elements.dropdownTipo.classList.remove('active');
            }
        });

        elements.dropdownTipoCheckboxes.forEach(cb => {
            cb.addEventListener('change', updateDropdownText);
        });

        updateDropdownText();
    }

    function updateDropdownText() {
        const selected = Array.from(elements.dropdownTipoCheckboxes).filter(cb => cb.checked);
        if (selected.length === 0) {
            elements.dropdownTipoText.textContent = 'Selecione os tipos...';
        } else if (selected.length === 1) {
            elements.dropdownTipoText.textContent = selected[0].nextElementSibling.textContent;
        } else {
            const names = selected.map(cb => cb.nextElementSibling.textContent);
            const last = names.pop();
            elements.dropdownTipoText.textContent = names.join(', ') + ' e ' + last;
        }
    }

    // ---- Utilitários ----

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

    function formatarCentavos(centavos) {
        const valor = (centavos / 100).toFixed(2);
        return valor.replace('.', ',').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
    }

    function converterParaCentavos(valorFormatado) {
        const valorLimpo = valorFormatado.replace(/\./g, '').replace(',', '.');
        return Math.round(parseFloat(valorLimpo) * 100);
    }

})();
