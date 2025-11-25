# Ticketing API – 活動 / 時段 / 訂單與退款系統

一個以 .NET 9 Minimal API 實作的活動售票後端，涵蓋活動管理、場次時段、座位、訂單、退款與行程通知等完整流程，採用分層架構與 EF Core（PostgreSQL）做持久化。

---

## 功能總覽

- 活動管理（Activities）
  - 建立 / 編輯 / 查詢活動
  - 依關鍵字、類別、地點搜尋
  - 活動上架 / 下架 / 狀態切換

- 時段管理（Timeslots）
  - 針對單一活動建立多個時段
  - 設定容量（capacity）
  - 調整時段狀態（可售 / 關閉等）

- 座位管理（Seats）
  - 查詢時段下所有座位
  - 標售 / 釋放座位（由訂單流程驅動）

- 訂單與明細（Orders）
  - 建立訂單（驗證活動 / 時段狀態、容量、座位）
  - 訂單明細（對應座位與時段）
  - 訂單狀態流轉：建立 → 已付（MVP 直接視為已付）→ 取消 / 退款中 / 已退款
  - 取消訂單並回補庫存 / 座位

- 退款流程（Refunds）
  - 以訂單為單位申請退款
  - 驗證退款金額不得超過已付金額
  - 退款狀態流轉：申請中 → 核准 / 駁回 → 完成
  - 完成退款時同步更新訂單狀態與已退款金額

- 行程提醒（Itinerary Notifications）
  - 為訂單排程行程提醒（例如活動前 24 小時）
  - 查詢與標記提醒為已發送

- 報表（Reports）
  - 活動銷售統計
  - 各時段銷量與剩餘座位

- API 文件
  - 內建 Swagger UI，可直接操作與測試所有端點

---

## 系統架構

專案採用簡化的 DDD 分層風格：

- **API（Ticketing.Api）**
  - `ActivityEndpoints.cs`：`/activities` 相關路由（搜尋 / CRUD / 狀態切換）
  - `TimeslotEndpoints.cs`：`/activities/{id}/timeslots`、`/timeslots/{id}` 等
  - `SeatEndpoints.cs`：查詢時段座位
  - `OrderEndpoints.cs`：建立 / 查詢 / 取消訂單
  - `RefundEndpoints.cs`：申請退款、審核（核准 / 駁回 / 完成）
  - `NotificationEndpoints.cs`：行程提醒查詢與重送排程
  - `ReportEndpoints.cs`：報表查詢
  - `ApiError.cs`：統一錯誤回應模型

- **Application（Ticketing.Application） – 流程 / 用例層**
  - `ActivityService`：活動 CRUD 與上架規則
  - `TimeslotService`：時段 CRUD 與狀態調整
  - `SeatService`：座位查詢與標售 / 釋放
  - `OrderService`：訂單建立（驗證活動 / 時段 / 庫存 / 座位）、取消與回補
  - `RefundService`：退款申請、審核、完成，並更新訂單與已退款金額
  - `NotificationService`：行程提醒排程與標記發送
  - `ReportService`：銷售與庫存報表

- **Domain（Ticketing.Domain） – 規則與狀態**
  - `Activity`：活動實體，含名稱、分類、地點與狀態
  - `Timeslot`：時段實體，容量 / 剩餘量與狀態（可售 / 關閉等）
  - `Seat`：座位實體，與時段關聯，控制標售 / 釋放
  - `Order`：訂單聚合根，包含明細、狀態機、已退款金額與取消 / 退款相關規則
  - `Refund`：退款實體，記錄申請金額與審核 / 完成狀態
  - `ItineraryNotification`：行程提醒紀錄
  - `Enums`：活動 / 時段 / 座位 / 訂單 / 退款 / 通知的狀態列舉

- **Infrastructure（Ticketing.Infrastructure） – 外部資源存取**
  - `TicketingDbContext`：EF Core DbContext
  - `Configurations/*`：各實體的 Fluent API 設定（表名、欄位、關聯、索引）
    - 包含 OrderNumber 唯一索引、OrderItem Timeslot 連結等
  - `Migrations/*`：所有 schema 遷移（包含新增 refundedAmount、order-item timeslot 與索引）

---

## 使用技術

- **後端框架**：.NET 9 Minimal API（C#）
- **ORM**：Entity Framework Core
- **資料庫**：PostgreSQL
- **文件**：Swagger / OpenAPI
- **架構風格**：分層架構（API / Application / Domain / Infrastructure），DDD-lite 風格聚合設計

---

## 專案啟動方式

1. **設定連線字串**

在 `appsettings.Development.json` 中設定 PostgreSQL 連線字串：

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=ticketing;Username=...;Password=..."
  }
}

在專案根目錄執行：
dotnet ef database update

啟動 API
dotnet run

啟動後終端機會顯示：
Now listening on: https://localhost:xxxx

開啟 Swagger UI
在瀏覽器中開啟：
https://localhost:xxxx/swagger

即可瀏覽並測試所有 API。
