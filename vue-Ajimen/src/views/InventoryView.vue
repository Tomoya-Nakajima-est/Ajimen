@model List<YourProject.Models.StockOrderViewModel>
    @{
    ViewBag.Title = "在庫・発注管理";
    }
    <h2>在庫・発注管理</h2>

    <!-- 商品一覧テーブル -->
    <table class="table">
        <thead>
            <tr>
                <th>ID</th>
                <th>商品名</th>
                <th>在庫数</th>
                <th>発注目安数</th>
                <th>発注</th>
                <th>編集</th>
            </tr>
        </thead>
        <tbody>
            @for (int i = 0; i < Model.Count; i++)
            {
            var item = Model[i];
            <tr>
                <td>@item.itemID</td>
                <td>@item.itemName</td>
                <!-- 在庫数：クリックでポップアップ -->
                <td>
                    <a href="#" onclick="openEditStockModal(@item.itemID, @item.stocNum); return false;">
                        @item.stocNum
                    </a>
                </td>
                <!-- 発注目安数：クリックでポップアップ -->
                <td>
                    <a href="#" onclick="openEditThresholdModal(@item.itemID, @item.threshold); return false;">
                        @item.threshold
                    </a>
                </td>
                <!-- 発注ボタン -->
                <td>
                    <button type="button" class="btn btn-success" onclick="openOrderModal(@item.itemID, '@item.itemName')">発注</button>
                </td>
                <!-- 編集ボタン -->
                <td>
                    <button type="button" class="btn btn-primary" onclick="openEditItemModal(@item.itemID)">編集</button>
                </td>
            </tr>
            }
        </tbody>
    </table>

    <!-- 商品新規追加ボタン -->
    <button class="btn btn-info" onclick="openAddItemModal()">商品新規追加</button>

    <!-- 発注予定商品リスト -->
    <h3 class="mt-4">発注予定商品リスト</h3>
    <table class="table" id="orderListTable">
        <thead>
            <tr>
                <th>ID</th>
                <th>商品名</th>
                <th>発注数</th>
                <th>削除</th>
            </tr>
        </thead>
        <tbody>
            <!-- JavaScriptで動的に行を追加 -->
        </tbody>
    </table>
    <!-- 発注内容確認・確定 -->
    <button class="btn btn-warning" onclick="openConfirmOrderModal()">発注内容確認・確定</button>

    <!-- 在庫数編集用モーダル -->
    <div class="modal fade" id="editStockModal" tabindex="-1">
        <div class="modal-dialog">
            <form id="editStockForm">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">在庫数編集</h5>
                    </div>
                    <div class="modal-body">
                        <input type="hidden" id="editStockItemID" />
                        <label>新しい在庫数:</label>
                        <input type="number" id="editStockNum" class="form-control" required min="0" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" onclick="confirmEditStock()">確認</button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">閉じる</button>
                    </div>
                </div>
            </form>
        </div>
    </div>