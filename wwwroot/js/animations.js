(function () {
    'use strict';

    const reduzirMovimento = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const PASSO_MS = 70;
    const DURACAO_CONTAGEM = 1100;

    if (!reduzirMovimento) animarContagens();
    configurarSaidaDePagina();

    function animarContagens() {
        const formato = new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

        document.querySelectorAll('[data-countup]').forEach(el => {
            const textoFinal = el.textContent;
            const m = textoFinal.match(/^([\s\S]*?)(-?[\d.]+,\d{2})([\s\S]*)$/);
            if (!m) return;

            const [, antes, numero, depois] = m;
            const alvo = parseFloat(numero.replace(/\./g, '').replace(',', '.'));
            if (!isFinite(alvo) || alvo === 0) return;

            el.textContent = antes + formato.format(0) + depois;

            setTimeout(() => {
                const inicio = performance.now();
                const passo = agora => {
                    const t = Math.min(1, (agora - inicio) / DURACAO_CONTAGEM);
                    const suavizado = 1 - Math.pow(1 - t, 3);
                    el.textContent = t < 1 ? antes + formato.format(alvo * suavizado) + depois : textoFinal;
                    if (t < 1) requestAnimationFrame(passo);
                };
                requestAnimationFrame(passo);
            }, atrasoDe(el) + 200);
        });
    }

    function atrasoDe(el) {
        const estilo = getComputedStyle(el);
        const bloco = parseInt(estilo.getPropertyValue('--anim-i')) || 0;
        const linha = el.closest('.conta-linha');
        const item = linha ? 1 + (parseInt(estilo.getPropertyValue('--item-i')) || 0) : 0;
        return (bloco + item) * PASSO_MS;
    }

    function configurarSaidaDePagina() {
        if (reduzirMovimento) return;

        document.querySelectorAll('#sidebar .nav-link').forEach(link => {
            link.addEventListener('click', e => {
                if (link.classList.contains('active') || e.ctrlKey || e.metaKey || e.shiftKey || e.button !== 0) return;
                e.preventDefault();
                document.body.classList.add('anim-saindo');
                setTimeout(() => { window.location.href = link.href; }, 160);
            });
        });

        window.addEventListener('pageshow', () => document.body.classList.remove('anim-saindo'));
    }
})();
