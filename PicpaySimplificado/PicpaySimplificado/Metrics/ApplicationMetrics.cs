using Prometheus;

namespace PicpaySimplificado.Metrics
{
    public static class ApplicationMetrics
    {
        // Contador de transferências realizadas
        public static readonly Counter TransferenciasRealizadas = Prometheus.Metrics
            .CreateCounter("picpay_transferencias_total", "Total de transferências realizadas",
                new CounterConfiguration
                {
                    LabelNames = new[] { "status" }
                });

        // Contador de transferências por tipo de usuário
        public static readonly Counter TransferenciasPorTipoUsuario = Prometheus.Metrics
            .CreateCounter("picpay_transferencias_por_tipo_usuario_total", "Total de transferências por tipo de usuário",
                new CounterConfiguration
                {
                    LabelNames = new[] { "tipo_usuario" }
                });

        // Histograma de valores de transferências
        public static readonly Histogram ValoresTransferencia = Prometheus.Metrics
            .CreateHistogram("picpay_transferencia_valor", "Distribuição de valores de transferências",
                new HistogramConfiguration
                {
                    Buckets = Histogram.LinearBuckets(10, 50, 10)
                });

        // Gauge para saldo total em carteiras
        public static readonly Gauge SaldoTotalCarteiras = Prometheus.Metrics
            .CreateGauge("picpay_saldo_total_carteiras", "Saldo total em todas as carteiras");

        // Contador de carteiras criadas
        public static readonly Counter CarteirasCriadas = Prometheus.Metrics
            .CreateCounter("picpay_carteiras_criadas_total", "Total de carteiras criadas",
                new CounterConfiguration
                {
                    LabelNames = new[] { "tipo_usuario" }
                });

        // Contador de autorizações negadas
        public static readonly Counter AutorizacoesNegadas = Prometheus.Metrics
            .CreateCounter("picpay_autorizacoes_negadas_total", "Total de autorizações negadas");

        // Contador de notificações enviadas
        public static readonly Counter NotificacoesEnviadas = Prometheus.Metrics
            .CreateCounter("picpay_notificacoes_enviadas_total", "Total de notificações enviadas",
                new CounterConfiguration
                {
                    LabelNames = new[] { "status" }
                });

        // Histograma de duração das transferências
        public static readonly Histogram DuracaoTransferencias = Prometheus.Metrics
            .CreateHistogram("picpay_transferencia_duracao_segundos", "Duração das transferências em segundos");

        // Contador de erros de validação
        public static readonly Counter ErrosValidacao = Prometheus.Metrics
            .CreateCounter("picpay_erros_validacao_total", "Total de erros de validação",
                new CounterConfiguration
                {
                    LabelNames = new[] { "tipo_erro" }
                });

        // Gauge para quantidade de carteiras ativas
        public static readonly Gauge CarteirasAtivas = Prometheus.Metrics
            .CreateGauge("picpay_carteiras_ativas", "Quantidade de carteiras ativas no sistema");

        // Regras de cálculo sugeridas
        /*
        # Total de transferências
        picpay_transferencias_total

        # Taxa de transferências por segundo
        rate(picpay_transferencias_total[1m])

        # Transferências por status
        sum by (status) (picpay_transferencias_total)

        # Duração média das transferências
        rate(picpay_transferencia_duracao_segundos_sum[5m]) / rate(picpay_transferencia_duracao_segundos_count[5m])
        */
    }
}
