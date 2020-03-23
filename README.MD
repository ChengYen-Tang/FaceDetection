# 臉部偵測-[開發中]

隨著 COVID-19 的確診人數不斷增加，越來越多單位會要求進入公司或校園前需要量體溫並戴口罩。本專案目的是為了降低人力成本，在單位入口架設普通鏡頭及紅外線鏡頭，自動化對穿過入口的人進行快速檢查。

## 效果
![001](./Img/Mask.png =400x450)
![002](./Img/NoMask.png =400x450)

## 流程
1. 使用 OpenCV Dnn 臉部偵測模型擷取畫面中的臉部
1. 將擷取到的臉部照片切成9宮格，取得額頭區塊，將此區塊溫度平均
1. 將擷取到的臉部透過訓練的模型偵測是否有戴口罩

## ToDo
- [ ] 臉部偵測模組
    - [ ] 單元測試
    - [x] Winform 效果檢視
- [ ] 口罩偵測模組
    - [ ] 單元測試
    - [x] Winform 效果檢視
- [ ] 溫度檢測模組
    - [ ] 單元測試
    - [ ] Winform 效果檢視
- [ ] 整合成 WebAPI
    - [ ] 單元測試

## 口罩訓練
- [訓練程式碼](https://github.com/ChengYen-Tang/DisasterClassification) (備註: 這是訓練其他東西用的，需要調整)
- [訓練資料集](https://drive.google.com/file/d/1meHXviN_PrWCOlaQRr7weNjr9g_xDqr-/view?usp=sharing)

## 已知問題
- 口罩偵測模組，某些角度或距離太遠可能會出現誤判的情況
    - 臨時解決方法
        1. 將員工或學生資料加入資料集( 訓練資料集出現過的人臉可解決此問題 )
    - 待實驗
        1. 使用較小型的神經網路
        1. 使用 [Facenet](https://github.com/davidsandberg/facenet) 取得面部特徵，並計算輸入與資料集的歐式距離來分類
        1. 增加訓練資料集資料
