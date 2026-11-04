(function () {
    'use strict';

    const { marcarCampo, enviarJson } = App;
    const $ = id => document.getElementById(id);

    const senha = {
        modal: $('resetSenhaModal'),
        id: $('resetSenhaId'),
        nome: $('resetSenhaNome'),
        campo: $('novaSenha'),
        confirmar: $('btnConfirmarResetSenha')
    };

    document.querySelectorAll('.btn-alterar-role').forEach(botao => botao.addEventListener('click', () => {
        const alternador = botao.closest('.dropdown').querySelector('.dropdown-toggle');
        enviarJson('/Usuarios/AlterarRole', { Id: botao.dataset.id, Role: botao.dataset.role }, {
            botao: alternador, sucesso: 'Permissão alterada com sucesso!'
        });
    }));

    document.querySelectorAll('.btn-toggle-ativo').forEach(botao => botao.addEventListener('click', () => {
        enviarJson('/Usuarios/ToggleAtivo', botao.dataset.id, { botao, sucesso: 'Status alterado com sucesso!' });
    }));

    document.querySelectorAll('.btn-reset-senha').forEach(botao => botao.addEventListener('click', () => {
        senha.id.value = botao.dataset.id;
        senha.nome.textContent = botao.dataset.nome;
        senha.campo.value = '';
        senha.campo.classList.remove('is-invalid', 'is-valid');
        bootstrap.Modal.getOrCreateInstance(senha.modal).show();
    }));

    senha.confirmar.addEventListener('click', async () => {
        const novaSenha = senha.campo.value.trim();
        if (!marcarCampo(senha.campo, novaSenha.length >= 6)) return;

        const ok = await enviarJson('/Usuarios/ResetSenha', { Id: senha.id.value, NovaSenha: novaSenha }, {
            botao: senha.confirmar, carregando: 'Redefinindo...', sucesso: 'Senha redefinida com sucesso!', depois: () => {}
        });

        if (ok) {
            bootstrap.Modal.getInstance(senha.modal).hide();
            btnReset(senha.confirmar);
        }
    });
})();
