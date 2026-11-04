(function () {
    'use strict';

    const classesToast = {
        success: 'text-bg-success',
        error: 'text-bg-danger',
        warning: 'text-bg-warning',
        info: 'text-bg-info'
    };

    window.showToast = function (mensagem, tipo = 'success') {
        const toast = document.getElementById('toastNotification');
        const texto = document.getElementById('toastMessage');
        if (!toast || !texto) return;

        texto.textContent = mensagem;
        toast.classList.remove(...Object.values(classesToast), 'text-bg-primary');
        toast.classList.add(classesToast[tipo] || 'text-bg-primary');
        bootstrap.Toast.getOrCreateInstance(toast, { delay: 4000 }).show();
    };

    window.btnLoading = function (botao, texto) {
        botao._htmlOriginal = botao.innerHTML;
        botao.disabled = true;
        botao.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>${texto || 'Carregando...'}`;
    };

    window.btnReset = function (botao) {
        botao.disabled = false;
        if (botao._htmlOriginal !== undefined) botao.innerHTML = botao._htmlOriginal;
    };

    const Moeda = {
        mascara(valor) {
            const digitos = String(valor).replace(/\D/g, '');
            const numero = parseInt(digitos, 10) || 0;
            return (numero / 100).toFixed(2).replace('.', ',').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
        },
        paraCentavos(texto) {
            const limpo = (texto || '').replace(/\./g, '').replace(',', '.');
            return Math.round(parseFloat(limpo) * 100) || 0;
        },
        formatar(centavos) {
            return Moeda.mascara(Math.round(centavos));
        },
        exibir(centavos) {
            return (centavos / 100).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        },
        aplicarMascara(campo) {
            campo.addEventListener('input', () => { campo.value = Moeda.mascara(campo.value); });
        }
    };

    function marcarCampo(campo, valido) {
        campo.classList.toggle('is-invalid', !valido);
        campo.classList.toggle('is-valid', valido);
        return valido;
    }

    function limparValidacao(container) {
        container.querySelectorAll('.is-invalid, .is-valid').forEach(el => el.classList.remove('is-invalid', 'is-valid'));
    }

    async function enviarJson(url, corpo, { botao, carregando = 'Salvando...', sucesso, depois } = {}) {
        if (botao) btnLoading(botao, carregando);

        try {
            const resposta = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(corpo)
            });
            const resultado = await resposta.json();

            if (resultado.success) {
                if (sucesso) showToast(sucesso, 'success');
                setTimeout(depois || (() => location.reload()), 1000);
                return true;
            }

            showToast(resultado.message || 'Não foi possível concluir a operação.', 'error');
        } catch {
            showToast('Não foi possível falar com o servidor. Verifique sua conexão e tente novamente.', 'error');
        }

        if (botao) btnReset(botao);
        return false;
    }

    function configurarFiltros(urlLimpar) {
        const card = document.getElementById('filterCard');
        const cabecalho = document.getElementById('filterCardHeader');
        const corpo = document.getElementById('filterCardBody');
        const limpar = document.getElementById('btnLimparFiltros');

        cabecalho?.addEventListener('click', () => {
            card.classList.toggle('collapsed');
            cabecalho.classList.toggle('collapsed');
            corpo.classList.toggle('collapsed');
        });

        limpar?.addEventListener('click', () => { window.location.href = urlLimpar; });

        const dropdown = document.querySelector('.dropdown-tipo');
        if (!dropdown) return;

        const texto = dropdown.querySelector('.dropdown-tipo-text');
        const caixas = Array.from(dropdown.querySelectorAll('input[type="checkbox"]'));

        const atualizarTexto = () => {
            const nomes = caixas.filter(c => c.checked).map(c => c.nextElementSibling.textContent);
            if (nomes.length === 0) texto.textContent = 'Selecione os tipos...';
            else if (nomes.length === 1) texto.textContent = nomes[0];
            else texto.textContent = `${nomes.slice(0, -1).join(', ')} e ${nomes.at(-1)}`;
        };

        dropdown.querySelector('.dropdown-tipo-toggle').addEventListener('click', e => {
            e.preventDefault();
            dropdown.classList.toggle('active');
        });

        document.addEventListener('click', e => {
            if (!dropdown.contains(e.target)) dropdown.classList.remove('active');
        });

        caixas.forEach(c => c.addEventListener('change', atualizarTexto));
        atualizarTexto();
    }

    function hojeIso() {
        const d = new Date();
        const pad = n => String(n).padStart(2, '0');
        return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
    }

    function escapeHtml(texto) {
        const div = document.createElement('div');
        div.textContent = texto;
        return div.innerHTML;
    }

    function configurarMenuLateral() {
        const menu = document.getElementById('sidebar');
        const botao = document.getElementById('btnToggleSidebar');
        const icone = document.getElementById('toggleIconMobile');
        const fundo = document.getElementById('sidebarOverlay');
        if (!menu || !botao) return;

        const alternar = () => {
            if (window.innerWidth >= 768) return;
            const aberto = menu.classList.toggle('expanded');
            icone.classList.toggle('bi-list', !aberto);
            icone.classList.toggle('bi-x-lg', aberto);
            document.body.classList.toggle('sidebar-open', aberto);
            fundo?.classList.toggle('show', aberto);
            botao.setAttribute('aria-expanded', String(aberto));
        };

        botao.addEventListener('click', alternar);
        fundo?.addEventListener('click', alternar);
    }

    configurarMenuLateral();

    window.App = { Moeda, marcarCampo, limparValidacao, enviarJson, configurarFiltros, hojeIso, escapeHtml };
})();
