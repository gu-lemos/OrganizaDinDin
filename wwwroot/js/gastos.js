(function() {
    'use strict';

    let gastos = [];
    let formElements = {};
    let gastoIdToDelete = null;

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        gastos = window.gastosData || [];
        cacheElements();
        attachEventListeners();
    }

    function cacheElements() {
        formElements = {
            form: document.getElementById('gastoForm'),
            gastoId: document.getElementById('gastoId'),
            descricao: document.getElementById('descricao'),
            tipo: document.getElementById('tipo'),
            data: document.getElementById('data'),
            valor: document.getElementById('valor'),
            btnSalvar: document.getElementById('btnSalvarGasto'),
            btnNovoGasto: document.getElementById('btnNovoGasto'),
            modal: document.getElementById('gastoModal'),
            modalTitle: document.getElementById('gastoModalLabel'),
            deleteModal: document.getElementById('deleteModal'),
            btnConfirmarDelete: document.getElementById('btnConfirmarDelete'),
            btnLimparFiltros: document.getElementById('btnLimparFiltros'),
            filterForm: document.getElementById('filterForm'),
            filtroDataInicio: document.getElementById('filtroDataInicio'),
            filtroDataFim: document.getElementById('filtroDataFim')
        };
    }

    function attachEventListeners() {
        formElements.btnNovoGasto.addEventListener('click', handleNovoGasto);

        formElements.btnSalvar.addEventListener('click', handleSalvarGasto);

        formElements.btnConfirmarDelete.addEventListener('click', confirmarDelete);

        document.getElementById('gastosTableBody').addEventListener('click', handleTableClick);

        formElements.descricao.addEventListener('input', () => validateField('descricao'));
        formElements.tipo.addEventListener('change', () => validateField('tipo'));
        formElements.data.addEventListener('change', () => validateField('data'));
        formElements.valor.addEventListener('input', handleValorInput);
        formElements.valor.addEventListener('blur', () => validateField('valor'));

        formElements.modal.addEventListener('show.bs.modal', clearValidation);

        if (formElements.btnLimparFiltros) {
            formElements.btnLimparFiltros.addEventListener('click', limparFiltros);
        }
    }

    function handleTableClick(e) {
        const btn = e.target.closest('button');
        if (!btn) return;

        const gastoId = btn.dataset.gastoId;

        if (btn.classList.contains('btn-editar')) {
            handleEditarGasto(gastoId);
        } else if (btn.classList.contains('btn-deletar')) {
            handleDeletarGasto(gastoId);
        }
    }

    function handleNovoGasto() {
        formElements.modalTitle.textContent = 'Novo Gasto';
        formElements.form.reset();
        formElements.gastoId.value = '';
        formElements.valor.value = '0,00';
        clearValidation();
    }

    function handleEditarGasto(id) {
        const gasto = gastos.find(g => g.Id === id);
        if (!gasto) return;

        formElements.modalTitle.textContent = 'Editar Gasto';
        formElements.gastoId.value = gasto.Id;
        formElements.descricao.value = gasto.Descricao;
        formElements.tipo.value = gasto.Tipo;
        formElements.data.value = gasto.Data.split('T')[0];
        formElements.valor.value = formatarValor(gasto.Valor);

        clearValidation();

        const modal = new bootstrap.Modal(formElements.modal);
        modal.show();
    }

    function handleDeletarGasto(id) {
        gastoIdToDelete = id;
        const modal = new bootstrap.Modal(formElements.deleteModal);
        modal.show();
    }

    async function confirmarDelete() {
        if (!gastoIdToDelete) return;

        const modal = bootstrap.Modal.getInstance(formElements.deleteModal);
        modal.hide();

        try {
            const response = await fetch('/Gastos/Delete', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(gastoIdToDelete)
            });

            const data = await response.json();

            if (data.success) {
                showToast('Gasto deletado com sucesso!', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(data.message, 'error');
            }
        } catch (error) {
            showToast('Erro ao deletar gasto: ' + error, 'error');
        } finally {
            gastoIdToDelete = null;
        }
    }

    async function handleSalvarGasto() {
        if (!validateForm()) {
            showToast('Por favor, corrija os erros no formulário', 'error');
            return;
        }

        const id = formElements.gastoId.value;
        const gasto = {
            Id: id || null,
            Descricao: formElements.descricao.value.trim(),
            Tipo: parseInt(formElements.tipo.value),
            Data: formElements.data.value,
            Valor: converterParaCentavos(formElements.valor.value)
        };

        const url = id ? '/Gastos/Update' : '/Gastos/Create';

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(gasto)
            });

            const result = await response.json();

            if (result.success) {
                showToast(id ? 'Gasto atualizado com sucesso!' : 'Gasto criado com sucesso!', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(result.message, 'error');
            }
        } catch (error) {
            showToast('Erro ao salvar gasto: ' + error, 'error');
        }
    }

    function handleValorInput(e) {
        e.target.value = aplicarMascaraMoeda(e.target.value);
        validateField('valor');
    }

    function validateForm() {
        const validations = [
            validateField('descricao'),
            validateField('tipo'),
            validateField('data'),
            validateField('valor')
        ];

        return validations.every(v => v === true);
    }

    function validateField(fieldName) {
        const field = formElements[fieldName];
        let isValid = true;

        switch (fieldName) {
            case 'descricao':
                const length = field.value.trim().length;
                isValid = length >= 5 && length <= 200;
                break;

            case 'tipo':
                isValid = field.value !== '';
                break;

            case 'data':
                isValid = field.value !== '';
                break;

            case 'valor':
                const valorCentavos = converterParaCentavos(field.value);
                isValid = valorCentavos > 0;
                break;
        }

        if (isValid) {
            field.classList.remove('is-invalid');
            field.classList.add('is-valid');
        } else {
            field.classList.remove('is-valid');
            field.classList.add('is-invalid');
        }

        return isValid;
    }

    function clearValidation() {
        const fields = [
            formElements.descricao,
            formElements.tipo,
            formElements.data,
            formElements.valor
        ];

        fields.forEach(field => {
            field.classList.remove('is-invalid', 'is-valid');
        });
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

    function formatarValor(centavos) {
        return (centavos / 100).toFixed(2).replace('.', ',');
    }

    function limparFiltros() {
        window.location.href = '/Gastos/Index';
    }

})();
