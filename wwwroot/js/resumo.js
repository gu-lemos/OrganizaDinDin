(function() {
    'use strict';

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        const gastosPorTipo = window.gastosPorTipo || [];

        if (gastosPorTipo.length === 0) {
            console.warn('Nenhum dado de gastos disponível');
            return;
        }

        inicializarGraficos(gastosPorTipo);
    }

    function inicializarGraficos(gastosPorTipo) {
        const labels = gastosPorTipo.map(g => g.Tipo);
        const valores = gastosPorTipo.map(g => g.Total);

        const cores = [
            '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF',
            '#FF9F40', '#FF6384', '#C9CBCF', '#4BC0C0'
        ];

        criarGraficoBarras(labels, valores, cores);
        criarGraficoPizza(labels, valores, cores);
    }

    function criarGraficoBarras(labels, valores, cores) {
        const ctx = document.getElementById('chartPorTipo');
        if (!ctx) return;

        new Chart(ctx.getContext('2d'), {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Valor Total (R$)',
                    data: valores,
                    backgroundColor: cores,
                    borderColor: cores,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return 'R$ ' + value.toFixed(2);
                            }
                        }
                    }
                }
            }
        });
    }

    function criarGraficoPizza(labels, valores, cores) {
        const ctx = document.getElementById('chartPizza');
        if (!ctx) return;

        new Chart(ctx.getContext('2d'), {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    data: valores,
                    backgroundColor: cores,
                    borderColor: '#fff',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const label = context.label || '';
                                const value = context.parsed || 0;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = ((value / total) * 100).toFixed(1);
                                return label + ': R$ ' + value.toFixed(2) + ' (' + percentage + '%)';
                            }
                        }
                    }
                }
            }
        });
    }

})();
