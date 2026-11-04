(function () {
    'use strict';

    const { Moeda, marcarCampo, limparValidacao, enviarJson, configurarFiltros, hojeIso, escapeHtml } = App;

    const gastos = window.gastosData || [];
    const compromissos = window.compromissosData || [];
    const urlListagem = window.gastosIndexUrl || '/Gastos/Index';
    const $ = id => document.getElementById(id);

    const gasto = {
        modal: $('gastoModal'),
        titulo: $('gastoModalLabel'),
        form: $('gastoForm'),
        id: $('gastoId'),
        conta: $('compromisso'),
        dica: $('compromissoHint'),
        descricao: $('descricao'),
        tipo: $('tipo'),
        data: $('data'),
        valor: $('valor'),
        salvar: $('btnSalvarGasto')
    };

    const conta = {
        modal: $('compromissoModal'),
        titulo: $('compromissoModalLabel'),
        id: $('compromissoId'),
        descricao: $('compromissoDescricao'),
        tipo: $('compromissoTipo'),
        valorTotal: $('compromissoValorTotal'),
        parcelas: $('compromissoParcelas'),
        dataFim: $('compromissoDataFim'),
        salvar: $('btnSalvarCompromisso')
    };

    const exclusao = {
        modal: $('deleteModal'),
        mensagem: $('deleteModalMensagem'),
        detalhe: $('deleteModalDetalhe'),
        confirmar: $('btnConfirmarDelete'),
        alvo: null
    };

    const validacoes = {
        descricao: () => marcarCampo(gasto.descricao, gasto.descricao.value.trim().length >= 5),
        tipo: () => marcarCampo(gasto.tipo, gasto.tipo.value !== ''),
        data: () => marcarCampo(gasto.data, gasto.data.value !== ''),
        valor: () => marcarCampo(gasto.valor, Moeda.paraCentavos(gasto.valor.value) > 0),
        contaDescricao: () => marcarCampo(conta.descricao, conta.descricao.value.trim().length >= 2),
        contaTipo: () => marcarCampo(conta.tipo, conta.tipo.value !== ''),
        contaParcelas: () => {
            const v = conta.parcelas.value.trim();
            return marcarCampo(conta.parcelas, v === '' || (/^\d+$/.test(v) && +v >= 1 && +v <= 600));
        },
        contaTotalOuData: () => {
            const ok = Moeda.paraCentavos(conta.valorTotal.value) > 0 || conta.dataFim.value !== '';
            marcarCampo(conta.valorTotal, ok);
            return marcarCampo(conta.dataFim, ok);
        }
    };

    configurarFiltros(urlListagem);
    Moeda.aplicarMascara(gasto.valor);
    Moeda.aplicarMascara(conta.valorTotal);

    $('btnNovoGasto').addEventListener('click', () => abrirGasto());
    $('btnNovoCompromisso').addEventListener('click', () => abrirConta());
    gasto.salvar.addEventListener('click', salvarGasto);
    conta.salvar.addEventListener('click', salvarConta);
    exclusao.confirmar.addEventListener('click', confirmarExclusao);

    gasto.descricao.addEventListener('input', validacoes.descricao);
    gasto.tipo.addEventListener('change', validacoes.tipo);
    gasto.data.addEventListener('change', validacoes.data);
    gasto.valor.addEventListener('input', validacoes.valor);
    gasto.conta.addEventListener('change', () => aplicarConta(false));

    conta.descricao.addEventListener('input', validacoes.contaDescricao);
    conta.tipo.addEventListener('change', validacoes.contaTipo);
    conta.parcelas.addEventListener('input', validacoes.contaParcelas);
    conta.valorTotal.addEventListener('input', validacoes.contaTotalOuData);
    conta.dataFim.addEventListener('change', validacoes.contaTotalOuData);

    $('gastosTableBody').addEventListener('click', e => {
        const botao = e.target.closest('button');
        if (!botao) return;
        if (botao.classList.contains('btn-editar')) editarGasto(botao.dataset.gastoId);
        if (botao.classList.contains('btn-deletar')) pedirExclusao('gasto', botao.dataset.gastoId);
    });

    document.addEventListener('click', e => {
        const botao = e.target.closest('[data-acao]');
        if (!botao) return;

        const id = botao.dataset.id;
        switch (botao.dataset.acao) {
            case 'nova-conta': abrirConta(); break;
            case 'editar-conta': abrirConta(id); break;
            case 'excluir-conta': pedirExclusao('conta', id); break;
            case 'pagar-conta': abrirGasto(id); break;
        }
    });

    function abrirGasto(compromissoId = '') {
        gasto.titulo.textContent = compromissoId ? 'Registrar Pagamento' : 'Novo Gasto';
        gasto.form.reset();
        gasto.id.value = '';
        gasto.valor.value = '0,00';
        gasto.data.value = hojeIso();
        gasto.conta.value = compromissoId;
        limparValidacao(gasto.form);

        aplicarConta(true);
        bootstrap.Modal.getOrCreateInstance(gasto.modal).show();
    }

    function editarGasto(id) {
        const item = gastos.find(g => g.Id === id);
        if (!item) return;

        gasto.titulo.textContent = 'Editar Gasto';
        gasto.id.value = item.Id;
        gasto.descricao.value = item.Descricao;
        gasto.tipo.value = item.Tipo;
        gasto.data.value = item.Data.split('T')[0];
        gasto.valor.value = Moeda.formatar(item.Valor);
        gasto.conta.value = item.CompromissoId || '';
        limparValidacao(gasto.form);

        atualizarDica();
        bootstrap.Modal.getOrCreateInstance(gasto.modal).show();
    }

    function contaSelecionada() {
        return compromissos.find(c => c.Id === gasto.conta.value) || null;
    }

    function aplicarConta(trocarDescricao) {
        atualizarDica();
        const c = contaSelecionada();
        if (!c) return;

        gasto.tipo.value = c.Tipo;
        validacoes.tipo();

        if (trocarDescricao || gasto.descricao.value.trim() === '') {
            gasto.descricao.value = c.Descricao.length >= 5 ? c.Descricao : `Pagamento ${c.Descricao}`;
        }
    }

    function valorSugerido(c) {
        if (!c.TemValorTotal) return c.MediaRecente || 0;
        const restantes = (c.TotalParcelas || 0) - c.QuantidadePagamentos;
        return restantes > 0 && c.ValorRestante > 0 ? Math.round(c.ValorRestante / restantes) : 0;
    }

    function atualizarDica() {
        const c = contaSelecionada();
        const icone = '<i class="bi bi-info-circle me-1"></i>';

        if (!c) {
            gasto.dica.innerHTML = `${icone}Será registrado como <strong>parcela única</strong>.`
                + (compromissos.length ? ' Para numerar as parcelas e ver quanto falta, escolha uma conta.' : '');
            return;
        }

        const jaVinculado = gastos.some(g => g.Id === gasto.id.value && g.CompromissoId === c.Id);
        const partes = [];

        if (!jaVinculado) {
            const total = c.TotalParcelasEfetivo ? `/${c.ParcelasEstimadas ? '~' : ''}${c.TotalParcelasEfetivo}` : '';
            partes.push(`Será a parcela ${c.QuantidadePagamentos + 1}${total}`);
        }

        if (c.TemValorTotal) {
            partes.push(c.ValorRestante > 0
                ? `Falta R$ ${Moeda.exibir(c.ValorRestante)} de R$ ${Moeda.exibir(c.ValorTotal)}`
                : 'Conta já quitada');
        } else if (c.DataFim) {
            const [ano, mes] = c.DataFim.split('-');
            partes.push(`Até ${mes}/${ano}`);
        }

        gasto.dica.innerHTML = icone + escapeHtml(partes.join(' · '));

        const sugestao = jaVinculado ? 0 : valorSugerido(c);
        if (sugestao <= 0) return;

        const usar = document.createElement('button');
        usar.type = 'button';
        usar.className = 'btn btn-link btn-sm p-0 align-baseline d-block';
        usar.title = c.TemValorTotal ? 'Restante ÷ parcelas que faltam' : 'Média dos últimos pagamentos';
        usar.textContent = `Usar valor sugerido: R$ ${Moeda.exibir(sugestao)}`;
        usar.addEventListener('click', () => {
            gasto.valor.value = Moeda.formatar(sugestao);
            validacoes.valor();
        });
        gasto.dica.appendChild(usar);
    }

    function salvarGasto() {
        const valido = [validacoes.descricao, validacoes.tipo, validacoes.data, validacoes.valor].map(v => v()).every(Boolean);
        if (!valido) {
            showToast('Por favor, corrija os erros no formulário', 'error');
            return;
        }

        const id = gasto.id.value;
        enviarJson(id ? '/Gastos/Update' : '/Gastos/Create', {
            Id: id || null,
            Descricao: gasto.descricao.value.trim(),
            Tipo: parseInt(gasto.tipo.value, 10),
            Data: gasto.data.value,
            Valor: Moeda.paraCentavos(gasto.valor.value),
            CompromissoId: gasto.conta.value || null
        }, { botao: gasto.salvar, sucesso: id ? 'Gasto atualizado com sucesso!' : 'Gasto criado com sucesso!' });
    }

    function abrirConta(id) {
        const c = id ? compromissos.find(x => x.Id === id) : null;

        conta.titulo.textContent = c ? 'Editar Conta' : 'Nova Conta';
        conta.id.value = c ? c.Id : '';
        conta.descricao.value = c ? c.Descricao : '';
        conta.tipo.value = c ? c.Tipo : '';
        conta.valorTotal.value = c ? Moeda.formatar(c.ValorTotal) : '0,00';
        conta.parcelas.value = c && c.TotalParcelas ? c.TotalParcelas : '';
        conta.dataFim.value = c && c.DataFim ? c.DataFim : '';
        limparValidacao(conta.modal);

        bootstrap.Modal.getOrCreateInstance(conta.modal).show();
    }

    function salvarConta() {
        const valido = [validacoes.contaDescricao, validacoes.contaTipo, validacoes.contaParcelas, validacoes.contaTotalOuData]
            .map(v => v())
            .every(Boolean);

        if (!valido) {
            showToast('Por favor, corrija os erros no formulário', 'error');
            return;
        }

        const id = conta.id.value;
        const parcelas = conta.parcelas.value.trim();
        enviarJson(id ? '/Compromissos/Update' : '/Compromissos/Create', {
            Id: id || null,
            Descricao: conta.descricao.value.trim(),
            Tipo: parseInt(conta.tipo.value, 10),
            ValorTotal: Moeda.paraCentavos(conta.valorTotal.value),
            TotalParcelas: parcelas ? parseInt(parcelas, 10) : null,
            DataFim: conta.dataFim.value || null
        }, { botao: conta.salvar, sucesso: id ? 'Conta atualizada com sucesso!' : 'Conta criada com sucesso!' });
    }

    function pedirExclusao(tipo, id) {
        exclusao.alvo = { tipo, id };

        if (tipo === 'conta') {
            const c = compromissos.find(x => x.Id === id);
            exclusao.mensagem.textContent = `Tem certeza que deseja excluir a conta "${c ? c.Descricao : ''}"?`;
            exclusao.detalhe.textContent = 'Os pagamentos já registrados continuam na lista de gastos, apenas deixam de estar vinculados a ela.';
        } else {
            exclusao.mensagem.textContent = 'Tem certeza que deseja excluir este gasto?';
            exclusao.detalhe.textContent = 'Esta ação não pode ser desfeita.';
        }

        bootstrap.Modal.getOrCreateInstance(exclusao.modal).show();
    }

    async function confirmarExclusao() {
        if (!exclusao.alvo) return;

        const { tipo, id } = exclusao.alvo;
        const ehConta = tipo === 'conta';
        const filtrandoPelaConta = ehConta && new URLSearchParams(location.search).get('compromissoId') === id;

        await enviarJson(ehConta ? '/Compromissos/Delete' : '/Gastos/Delete', id, {
            botao: exclusao.confirmar,
            carregando: 'Excluindo...',
            sucesso: ehConta ? 'Conta excluída com sucesso!' : 'Gasto excluído com sucesso!',
            depois: filtrandoPelaConta ? () => location.assign(urlListagem) : undefined
        });

        exclusao.alvo = null;
    }
})();
