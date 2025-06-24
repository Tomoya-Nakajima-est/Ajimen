import axios from 'axios';

export async function login(id, password) {
  const res = await axios.post('http://localhost:5022/api/User/login', {
    id,
    password
  });

  // ここで返ってくる情報が：
  // { id: "0003", fullName: "柳井晴", role: "アルバイト" }
  const { id: userId, fullName, role } = res.data;

  // ローカルストレージに保存
  localStorage.setItem('userId', userId);
  localStorage.setItem('userFullName', fullName);
  localStorage.setItem('userRole', role);

  return res.data;
}