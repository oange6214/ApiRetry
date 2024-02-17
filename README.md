[WebAPI] .NET 7 - Polly  Fault Handling / Resiliency / Retry Policy / Circuit Breaker

如何增加 API 的彈性並減少錯誤可能性？

在這次討論中，我們將著重討論 Retry Policy（重試策略）、Circuit Breaker（斷路器）和 Time Out（逾時）。

當我們的 API 嘗試連接到任何第三方服務時，我們需要實現哪些策略以確保在第三方服務失敗時能夠成功連接？我們應該設定多少次重試？最佳實踐是什麼？另外，在連續失敗的情況下，我們應該考慮切換到不同的第三方服務嗎？


未來規劃的專案結構：

```
ApiRetry/
│
├── src/
│   ├── ApiRetry.Core/
│   ├── ApiRetry.Infrastructure/
│   ├── ApiRetry.Web/
│   └── ApiRetry.Tests/
│
├── docs/
├── tools/                        # 專案用的工具
├── docker/
│   ├── Dockerfile                # 應用程式容器化配置檔
│   └── docker-compose.yml        # Docker Compose 配置檔
│
├── .gitignore
├── ApiRetry.sln
└── README.md
```

