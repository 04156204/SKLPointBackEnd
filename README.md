
<h2>點數後台 </h2><br>
請在本機新建一個專案，依資料庫規格開出localDB，並以分層式架構開發（需有Repository及service）。 <br>
<h3>功能 </h3><br>
1.	點數匯入機制 <br>
可輸入欲匯入的客戶ID、欲匯入點數、來源備註輸入框，匯入時需寫入LOG <br>
PS.DB的Source為Admin(後台帳號-可先建假的)、點數過期日為匯入當下+365天 <br>
 <br>
2.	點數查詢機制 <br>
a.	查詢點數資料：可用客戶ID查詢(必填)，也可用點數來源查詢(選填)。 <br>
先列出客戶之所有點數及到期點數(需與呈現給客戶數值相同，如圖)，再列出DB資料。 <br>
所有點數：依點數LOG Table統計客戶點數。 <br>
到期點數：查看可用點數的到期日，並依序列出（負數即不顯示）。 <br>
b.	查詢點數LOG資料：用客戶ID查詢。 <br>
 <br>
3.	點數註銷機制 <br>
輸入客戶ID及點數來源搜尋，列出所有明細，後面顯示註銷鍵，按下即觸發，並且收回相對應可用點數，紀錄LOG。 <br>
 <br>
4.	兌換點數機制 <br>
輸入客戶ID及兌換點數，執行相對應動作（扣點、寫LOG..等等）。 <br>
