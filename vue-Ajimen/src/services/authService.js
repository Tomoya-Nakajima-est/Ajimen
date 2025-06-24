import axios from 'axios'

export async function login(id, password) {
  const res = await axios.post('http://localhost:5022/api/User/login', { //ここをトンネル使うために変えています
    id,
    password
  });
  return res.data;
}