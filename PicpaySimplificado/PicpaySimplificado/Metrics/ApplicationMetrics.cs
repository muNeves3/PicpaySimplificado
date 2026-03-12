using Prometheus;

namespace PicpaySimplificado.Metrics
{
    public static class ApplicationMetrics
    {
        // Contador de transfer�ncias realizadas
        public static readonly Counter TransferenciasRealizadas = Prometheus.Metrics
            .CreateCounter("picpay_transferencias_total", "Total de transferências realizadas",
                new CounterConfiguration
                {
                    LabelNames = new[] { "status" }
                });

        // Contador de transfer�ncias por tipo de usu�rio
        public static readonly Counter TransferenciasPorTipoUsuario = Prometheus.Metrics
            .CreateCounter("picpay_transferencias_por_tipo_usuario_total", "Total de transferências por tipo de usuário",
                new CounterConfiguration
                {
                    LabelNames = new[] { "tipo_usuario" }
                });

        // Histograma de valores de transfer�ncias
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

        // Contador de autoriza��es negadas
        public static readonly Counter AutorizacoesNegadas = Prometheus.Metrics
            .CreateCounter("picpay_autorizacoes_negadas_total", "Total de autorizações negadas");

        // Contador de notifica��es enviadas
        public static readonly Counter NotificacoesEnviadas = Prometheus.Metrics
            .CreateCounter("picpay_notificacoes_enviadas_total", "Total de notificações enviadas",
                new CounterConfiguration
                {
                    LabelNames = new[] { "status" }
                });

        // Histograma de dura��o das transfer�ncias
        public static readonly Histogram DuracaoTransferencias = Prometheus.Metrics
            .CreateHistogram("picpay_transferencia_duracao_segundos", "Duração das transfer�ncias em segundos");

        // Contador de erros de valida��o
        public static readonly Counter ErrosValidacao = Prometheus.Metrics
            .CreateCounter("picpay_erros_validacao_total", "Total de erros de validação",
                new CounterConfiguration
                {
                    LabelNames = new[] { "tipo_erro" }
                });

        // Gauge para quantidade de carteiras ativas
        public static readonly Gauge CarteirasAtivas = Prometheus.Metrics
            .CreateGauge("picpay_carteiras_ativas", "Quantidade de carteiras ativas no sistema");

        // Regras de c�lculo sugeridas
        /*
        # Total de transfer�ncias
        picpay_transferencias_total

        # Taxa de transfer�ncias por segundo
        rate(picpay_transferencias_total[1m])

        # Transfer�ncias por status
        sum by (status) (picpay_transferencias_total)

        # Dura��o m�dia das transfer�ncias
        rate(picpay_transferencia_duracao_segundos_sum[5m]) / rate(picpay_transferencia_duracao_segundos_count[5m])
        */
    }
}
