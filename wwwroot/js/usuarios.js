(function() {
    'use strict';

    let elements = {};

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        cacheElements();
        attachEventListeners();
    }

    function cacheElements() {
        elements = {
            resetSenhaModal: document.getElementById('resetSenhaModal'),
            resetSenhaId: document.getElementById('resetSenhaId'),
            resetSenhaNome: document.getElementById('resetSenhaNome'),
            novaSenha: document.getElementById('novaSenha'),
            resetSenhaForm: document.getElementById('resetSenhaForm'),
            btnConfirmarResetSenha: document.getElementById('btnConfirmarResetSenha')
        };
    }

    function attachEventListeners() {
        // Alterar Role
        document.querySelectorAll('.btn-alterar-role').forEach(btn => {
            btn.addEventListener('click', handleAlterarRole);
        });

        // Toggle Ativo
        document.querySelectorAll('.btn-toggle-ativo').forEach(btn => {
            btn.addEventListener('click', handleToggleAtivo);
        });

        // Reset Senha - abrir modal
        document.querySelectorAll('.btn-reset-senha').forEach(btn => {
            btn.addEventListener('click', function() {
                elements.resetSenhaId.value = this.dataset.id;
                elements.resetSenhaNome.textContent = this.dataset.nome;
                elements.novaSenha.value = '';
                elements.novaSenha.classList.remove('is-invalid');
                var modal = new bootstrap.Modal(elements.resetSenhaModal);
                modal.show();
            });
        });

        // Confirmar Reset Senha
        elements.btnConfirmarResetSenha.addEventListener('click', handleResetSenha);
    }

    async function handleAlterarRole(e) {
        var btn = e.currentTarget;
        var id = btn.dataset.id;
        var role = btn.dataset.role;

        var dropdownBtn = btn.closest('.dropdown').querySelector('.dropdown-toggle');
        btnLoading(dropdownBtn, 'Salvando...');

        try {
            var response = await fetch('/Usuarios/AlterarRole', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: id, role: role })
            });

            var result = await response.json();

            if (result.success) {
                showToast('Role alterada com sucesso!', 'success');
                setTimeout(function() { location.reload(); }, 1000);
            } else {
                showToast(result.message, 'error');
                btnReset(dropdownBtn);
            }
        } catch (error) {
            showToast('Erro ao alterar role: ' + error, 'error');
            btnReset(dropdownBtn);
        }
    }

    async function handleToggleAtivo(e) {
        var btn = e.currentTarget;
        var id = btn.dataset.id;

        btnLoading(btn, 'Salvando...');

        try {
            var response = await fetch('/Usuarios/ToggleAtivo', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(id)
            });

            var result = await response.json();

            if (result.success) {
                showToast('Status alterado com sucesso!', 'success');
                setTimeout(function() { location.reload(); }, 1000);
            } else {
                showToast(result.message, 'error');
                btnReset(btn);
            }
        } catch (error) {
            showToast('Erro ao alterar status: ' + error, 'error');
            btnReset(btn);
        }
    }

    async function handleResetSenha() {
        var novaSenha = elements.novaSenha.value.trim();

        if (novaSenha.length < 6) {
            elements.novaSenha.classList.add('is-invalid');
            return;
        }

        elements.novaSenha.classList.remove('is-invalid');
        btnLoading(elements.btnConfirmarResetSenha, 'Resetando...');

        var dto = {
            id: elements.resetSenhaId.value,
            novaSenha: novaSenha
        };

        try {
            var response = await fetch('/Usuarios/ResetSenha', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(dto)
            });

            var result = await response.json();

            if (result.success) {
                showToast('Senha resetada com sucesso!', 'success');
                bootstrap.Modal.getInstance(elements.resetSenhaModal).hide();
                btnReset(elements.btnConfirmarResetSenha);
            } else {
                showToast(result.message, 'error');
                btnReset(elements.btnConfirmarResetSenha);
            }
        } catch (error) {
            showToast('Erro ao resetar senha: ' + error, 'error');
            btnReset(elements.btnConfirmarResetSenha);
        }
    }

})();
