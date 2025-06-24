<template>
  <div class="home-root">
    <!-- サイドバー（省略。上記デザイン参照） -->
    <aside class="sidebar">
      <div class="logo">&lt;AJIQ&gt;</div>
      <div class="menu-title">－MENU－</div>
      <button class="menu-btn">HOME</button>
      <button class="menu-btn">シフト表</button>
      <button class="menu-btn active">発注・在庫</button>
    </aside>

    <main class="main-area">
      <table class="table">
  <thead>
    <tr>
      <th>在庫ログID</th>
      <th>登録者ID</th>
      <th>登録者名</th>
      <th>記録日</th>
      <th>在庫数</th>
      <th>最低在庫数</th>
      <th>物品ID</th>
      <th>物品名</th>
      <th>物品の種類</th>
      <th>発注</th>
    </tr>
  </thead>
  <tbody>
    <tr v-for="stock in stockList" :key="stock.stockLogId">
      <td>{{ stock.stockLogId }}</td>
      <td>{{ stock.staffId }}</td>
      <td>{{ stock.staffName }}</td>
      <td>{{ new Date(stock.stockDay).toLocaleString() }}</td>
      <td>{{ stock.stockNum }}</td>
      <td>{{ stock.stockMinNum }}</td>
      <td>{{ stock.itemId }}</td>
      <td>{{ stock.itemName }}</td>
      <td>{{ stock.itemKinds }}</td>
      <td>
        <button @click="addToOrderList(stock)">発注</button>
      </td>
    </tr>
  </tbody>
</table>


      <!-- 発注予定 -->
      <section class="order-area">
        <h2 class="section-title">－発注予定－</h2>
        <div class="order-table-wrap">
          <table class="table" id="orderListTable">
  <thead>
    <tr>
      <th>OrderLogId</th>
      <th>StaffId</th>
      <th>StaffName</th>
      <th>ItemId</th>
      <th>ItemName</th>
      <th>ItemKinds</th>
      <th>OrderNum</th>
      <th>削除</th>
    </tr>
  </thead>
  <tbody>
    <tr v-for="order in orderList" :key="order.ItemId">
      <td>{{ order.OrderLogId }}</td>
      <td>{{ order.StaffId }}</td>
      <td>{{ order.StaffName }}</td>
      <td>{{ order.ItemId }}</td>
      <td>{{ order.ItemName }}</td>
      <td>{{ order.ItemKinds }}</td>
      <td>{{ order.OrderNum }}</td>
      <td>
        <button @click="removeOrder(order.ItemId)">削除</button>
      </td>
    </tr>
  </tbody>
</table>
        </div>
        <div class="order-btn-row">
          <button class="order-btn-big" @click="submitOrders" :disabled="orderList.length === 0">発注</button>
          <button class="cancel-btn-big" @click="resetOrderList">取消</button>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const stockList = ref([])
const allStockList = ref([])
const orderList = ref([])
const searchKeyword = ref('')

// 在庫データ取得
onMounted(async () => {
  const res = await axios.get('http://localhost:5022/api/stock/logs')
  stockList.value = res.data
  allStockList.value = res.data // 検索リセット用
})

// 例：発注リストに商品を追加
function addToOrderList(item) {
  const orderNum = prompt("発注数を入力してください", 1)
  if (orderNum !== null && !isNaN(Number(orderNum))) {
    // すでに同じ商品がリストにある場合は更新
    const existing = orderList.value.find(o => o.ItemId === item.itemID)
    const orderData = {
      OrderLogId: 0, // 新規作成なので0またはnull
      StaffId: "0001", // 実際はログインユーザーから取得
      StaffName: "発注太郎", // 実際はログインユーザーから取得
      OrderNum: Number(orderNum),
      ItemId: item.itemID,
      ItemName: item.itemName,
      ItemKinds: item.itemKinds || "未分類",
      // OrderDayとuseOrderLogはAPIでセットされるので未指定
    }
    if (existing) {
      Object.assign(existing, orderData)
    } else {
      orderList.value.push(orderData)
    }
  }
}

function removeOrder(itemId) {
  orderList.value = orderList.value.filter(o => o.itemId !== itemId)
}

function resetOrderList() {
  orderList.value = []
}

function searchStock() {
  if (!searchKeyword.value) {
    stockList.value = allStockList.value
  } else {
    stockList.value = allStockList.value.filter(
      s => s.itemName && s.itemName.includes(searchKeyword.value)
    )
  }
}

// 発注データ送信API例
async function submitOrders() {
  if (orderList.value.length === 0) return
  try {
    const res = await axios.post('http://localhost:5022/api/orderlog/submitandmail', orderList.value)
    // 成功時のハンドリング
    if (res.status === 200) {
      alert("発注内容を保存し、メール送信に成功しました。\n" + (res.data || ""));
      orderList.value = []
    } else {
      alert("サーバーがエラー応答を返しました: " + (res.data || "詳細不明"))
    }
  } catch (e) {
    // エラー時のハンドリング
    console.log(JSON.stringify(orderList.value));
    let msg = "保存またはメール送信時にエラーが発生しました。"
    if (e.response && e.response.data) {
      msg += "\n詳細: " + e.response.data
    }
    alert(msg)
  }
}

</script>

<style scoped>
body {
  background: #75022d;
}
.home-root {
  display: flex;
  min-height: 100vh;
  background: #75022d;
  /* 追加↓ */
  width: 100vw;
  box-sizing: border-box;
}
.sidebar {
  min-width: 180px;
  max-width: 280px;
  width: 18vw;
  background: #f3ede6;
  border-radius: 0 0 0 0;
  margin: 2vw 0 2vw 2vw;
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  box-shadow: 0 4px 16px rgba(90,0,40,0.08);
  height: calc(100vh - 4vw);
}
.main-area {
  flex: 1;
  margin: 2vw 2vw 2vw 1.2vw;
  padding: 2vw;
  background: #f3ede6;
  border-radius: 0.75rem;
  min-width: 360px;
  box-shadow: 0 2px 16px rgba(90,0,40,0.08);
  /* 追加↓ */
  overflow-x: auto;
}
.stock-table-wrap, .order-table-wrap {
  width: 98%;
  margin: 0 auto 1vw auto;
  overflow-x: auto;
}
.table {
  width: 100%;
  border-collapse: collapse;
  font-size: 1.02rem;
  margin-bottom: 8px;
  /* 追加↓ */
  min-width: 700px;
}
th, td {
  word-break: break-all;
}
.search-row,
.order-btn-row {
  flex-wrap: wrap;
  gap: 1vw;
}
@media (max-width: 1000px) {
  .home-root {
    flex-direction: column;
  }
 .sidebar {
  width: 240px;
  background: #f3ede6;
  /* margin: 24px 0 24px 56px; ←これを↓に変更 */
  margin: 24px 0 24px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  transition: transform 0.3s;
  z-index: 1000;
}

.main-area {
  flex: 1;
  /* margin: 24px 48px 24px 32px; ←これを↓に変更 */
  margin: 24px 48px 24px 0;
  padding: 24px 36px 32px 36px;
  background: #f3ede6;
  border-radius: 0.75rem;
  min-width: 320px;
  box-shadow: 0 2px 16px rgba(90,0,40,0.08);
  transition: width 0.3s;
}
}
@media (max-width: 700px) {
  .main-area,
  .sidebar {
    padding: 2vw 1vw;
  }
  .table {
    font-size: 0.9rem;
    min-width: 380px;
  }
}
.logo {
  font-size: 2.1rem;
  margin: 32px 0 8px 0;
  color: #6b2236;
  font-family: serif;
  text-align: center;
  letter-spacing: 0.08em;
}
.menu-title {
  font-size: 1.7rem;
  margin: 20px 0 10px 0;
  color: #6b2236;
  text-align: center;
  font-family: serif;
  letter-spacing: 0.06em;
}
.menu-btn {
  width: 180px;
  margin: 8px auto;
  padding: 12px 0;
  background: #fff;
  border: 1px solid #d9d9d9;
  border-radius: 3px;
  font-size: 1.2rem;
  color: #3a2634;
  cursor: pointer;
  transition: background 0.15s;
  font-family: 'Noto Serif JP', serif;
}
.menu-btn.active {
  background: #d2d6da;
  color: #6b2236;
  font-weight: bold;
  border: 2px solid #b0b0b0;
}
.footer-img {
  width: 60px;
  position: absolute;
  bottom: 16px;
  left: 16px;
}

.main-area {
  flex: 1;
  margin: 24px 48px 24px 32px;
  padding: 24px 36px 32px 36px;
  background: #f3ede6;
  border-radius: 0.75rem;
  min-width: 700px;
  box-shadow: 0 2px 16px rgba(90,0,40,0.08);
}

.section-title {
  font-family: 'Yu Mincho', serif;
  font-size: 2.2rem;
  text-align: center;
  color: #555;
  margin: 8px 0 18px 0;
  letter-spacing: 0.12em;
}

.stock-table-wrap, .order-table-wrap {
  width: 93%;
  margin: 0 auto 12px auto;
}
.table {
  width: 100%;
  border-collapse: collapse;
  font-size: 1.02rem;
  margin-bottom: 8px;
}
.table thead tr {
  background: #1a1113;
  color: #fff;
}
.table th, .table td {
  padding: 8px 12px;
  border: 1px solid #b8a8ad;
  text-align: center;
}
.table td {
  background: #fff;
}
.order-btn-table {
  background: #fafafa;
  border: 1px solid #b7d1e5;
  border-radius: 8px;
  padding: 2px 20px;
  color: #6b2236;
  font-weight: bold;
  font-size: 1.05rem;
  cursor: pointer;
  transition: background 0.15s;
}
.order-btn-table:hover {
  background: #eaf8ff;
}
.cancel-btn-table {
  background: #fff;
  border: 1px solid #cbb4ce;
  border-radius: 8px;
  padding: 2px 20px;
  color: #94618e;
  font-weight: bold;
  font-size: 1.05rem;
  cursor: pointer;
  transition: background 0.15s;
}
.cancel-btn-table:hover {
  background: #fbe5fa;
}
.search-row {
  margin: 8px auto 0 auto;
  width: 70%;
  display: flex;
  justify-content: flex-end;
}
.search-input {
  border: 1.5px solid #aaa;
  border-radius: 18px;
  padding: 8px 18px;
  font-size: 1.1rem;
  width: 210px;
  margin-right: 12px;
}
.search-btn {
  border-radius: 18px;
  padding: 7px 24px;
  border: 1px solid #7e4b5b;
  background: #fff;
  color: #6b2236;
  font-size: 1.15rem;
  cursor: pointer;
  transition: background 0.18s;
}
.search-btn:hover {
  background: #ffe3ee;
}
.order-btn-row {
  margin: 18px auto 0 auto;
  display: flex;
  justify-content: center;
  gap: 24px;
}
.order-btn-big {
  background: #eaf8ff;
  color: #486578;
  font-size: 1.7rem;
  padding: 10px 54px;
  border: 1.5px solid #b7d1e5;
  border-radius: 12px;
  cursor: pointer;
  transition: background 0.18s;
}
.cancel-btn-big {
  background: #edd8ed;
  color: #94618e;
  font-size: 1.7rem;
  padding: 10px 54px;
  border: 1.5px solid #cbb4ce;
  border-radius: 12px;
  cursor: pointer;
  transition: background 0.18s;
}
.order-btn-big:hover { background: #d2eefd; }
.cancel-btn-big:hover { background: #fbe5fa; }
</style>

