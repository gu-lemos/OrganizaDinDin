(function () {
    'use strict';

    const { Moeda, marcarCampo, limparValidacao, enviarJson, configurarFiltros, hojeIso } = App;
    const $ = id => document.getElementById(id);
    const TIPO_RESGATE = 1;

    const formularios = {
        deposito: {
            modal: $('depositoModal'), form: $('depositoForm'), confirmar: $('btnConfirmarDeposito'),
            valor: $('depositoValor'), data: $('depositoData'), usuario: $('depositoUsuario')
        },
        resgate: {
            modal: $('resgateModal'), form: $('resgateForm'), confirmar: $('btnConfirmarResgate'),
            valor: $('resgateValor'), data: $('resgateData'), usuario: $('resgateUsuario'), motivo: $('resgateMotivo')
        },
        editar: {
            modal: $('editarModal'), form: $('editarForm'), confirmar: $('btnConfirmarEditar'),
            id: $('editarId'), tipo: $('editarTipo'), valor: $('editarValor'), data: $('editarData'),
            usuario: $('editarUsuario'), motivo: $('editarMotivo'), grupoMotivo: $('editarMotivoGroup')
        },
        meta: {
            modal: $('metaModal'), form: $('metaForm'), confirmar: $('btnConfirmarMeta'), valor: $('metaValor')
        },
        excluir: {
            modal: $('excluirModal'), confirmar: $('btnConfirmarExcluir'), id: $('excluirId'), descricao: $('excluirDescricao')
        }
    };

    const { deposito, resgate, editar, meta, excluir } = formularios;

    configurarFiltros('/Cofrinho/Index');
    [deposito, resgate, editar, meta].forEach(f => Moeda.aplicarMascara(f.valor));

    [deposito, resgate].forEach(f => f.modal.addEventListener('show.bs.modal', () => {
        f.form.reset();
        f.valor.value = '0,00';
        f.data.value = hojeIso();
        limparValidacao(f.form);
    }));
    meta.modal.addEventListener('show.bs.modal', () => limparValidacao(meta.form));

    editar.tipo.addEventListener('change', () => mostrarMotivo(parseInt(editar.tipo.value, 10)));

    deposito.confirmar.addEventListener('click', () => {
        if (!validarMovimentacao(deposito)) return;
        enviarJson('/Cofrinho/Depositar', dadosMovimentacao(deposito), {
            botao: deposito.confirmar, carregando: 'Depositando...', sucesso: 'Depósito realizado com sucesso!'
        });
    });

    resgate.confirmar.addEventListener('click', () => {
        if (!validarMovimentacao(resgate)) return;
        enviarJson('/Cofrinho/Resgatar', { ...dadosMovimentacao(resgate), Motivo: resgate.motivo.value.trim() || null }, {
            botao: resgate.confirmar, carregando: 'Resgatando...', sucesso: 'Resgate realizado com sucesso!'
        });
    });

    editar.confirmar.addEventListener('click', () => {
        if (!validarMovimentacao(editar)) return;
        enviarJson('/Cofrinho/Editar', {
            ...dadosMovimentacao(editar),
            Id: editar.id.value,
            Tipo: parseInt(editar.tipo.value, 10),
            Motivo: editar.motivo.value.trim() || null
        }, { botao: editar.confirmar, sucesso: 'Movimentação atualizada com sucesso!' });
    });

    meta.confirmar.addEventListener('click', () => {
        if (!marcarCampo(meta.valor, Moeda.paraCentavos(meta.valor.value) > 0)) {
            showToast('Informe um valor válido para a meta', 'error');
            return;
        }
        enviarJson('/Cofrinho/SetMeta', { Valor: Moeda.paraCentavos(meta.valor.value) }, {
            botao: meta.confirmar, sucesso: 'Meta definida com sucesso!'
        });
    });

    excluir.confirmar.addEventListener('click', () => {
        if (!excluir.id.value) return;
        enviarJson('/Cofrinho/Excluir', { Id: excluir.id.value }, {
            botao: excluir.confirmar, carregando: 'Excluindo...', sucesso: 'Movimentação excluída com sucesso!'
        });
    });

    document.addEventListener('click', e => {
        const botaoEditar = e.target.closest('.btn-editar');
        if (botaoEditar) {
            abrirEdicao(botaoEditar.dataset);
            return;
        }

        const botaoExcluir = e.target.closest('.btn-excluir');
        if (botaoExcluir) {
            excluir.id.value = botaoExcluir.dataset.id;
            excluir.descricao.textContent = botaoExcluir.dataset.descricao;
            bootstrap.Modal.getOrCreateInstance(excluir.modal).show();
        }
    });

    function abrirEdicao(dados) {
        const tipo = parseInt(dados.tipo, 10);

        editar.id.value = dados.id;
        editar.tipo.value = tipo;
        editar.valor.value = Moeda.formatar(parseInt(dados.valor, 10));
        editar.data.value = dados.data;
        editar.usuario.value = dados.usuario;
        editar.motivo.value = dados.motivo || '';
        mostrarMotivo(tipo);
        limparValidacao(editar.form);

        bootstrap.Modal.getOrCreateInstance(editar.modal).show();
    }

    function mostrarMotivo(tipo) {
        editar.grupoMotivo.style.display = tipo === TIPO_RESGATE ? 'block' : 'none';
    }

    function validarMovimentacao(f) {
        const valido = [
            marcarCampo(f.valor, Moeda.paraCentavos(f.valor.value) > 0),
            marcarCampo(f.data, f.data.value !== ''),
            marcarCampo(f.usuario, f.usuario.value !== '')
        ].every(Boolean);

        if (!valido) showToast('Por favor, corrija os erros no formulário', 'error');
        return valido;
    }

    function dadosMovimentacao(f) {
        return {
            Valor: Moeda.paraCentavos(f.valor.value),
            Data: f.data.value,
            UsuarioId: f.usuario.value
        };
    }
})();
