|[中文](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards)| |[EN](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-EN.md)| |[Русский](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-RU.md)| |[日本語](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-JP.md)| |[Español](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-ES.md)|

> Use AI Translation

# VRCビリヤード - リーダーボード - スヌーカー
## プロジェクト継承テーブル
* #### MS-VRCSA-Billiards
  * #### VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards [ランドマークの更新](https://github.com/WangQAQ/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards)
  	  * ### VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards (現在)
### このテーブルの目的は、エンターテイメント向けの体験を提供し、プラグインや楽しい機能を追加することです。
#### [著作権テーブル](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/Copyright.md)

> このテーブルで遊ぶことができます：[マップ](https://vrchat.com/home/world/wrld_d9ac19bc-a8c4-42cd-b712-c66dd813bd8c/info)
---
## 1. 新機能
* ### アップロードパネル:
	* #### もうStart関数でタスクをバインドする必要はありません。現在のバインディングはEditTimeで完了し、追加のパフォーマンスコストはありません。
	* #### アップロード方法が簡略化され、バインディングを心配することなく、パネルで順番にクリックするだけでバインディングを完了できます。
	* #### リーダーボードキー管理システムの改善。自動リーダーボードを使用していると、操作によってキーが失われることがありますが、今では新しいユーザーシステムを作成し、キーをバインドできるようになりました。
* ### ユーザーシステム:
	* #### [アカウント](https://www.wangqaq.com/PoolBar/Account)でアカウントを登録できます。
	* #### 登録後、WEBでカラフルな名前を選択できます。
	* #### 現在も匿名でキーをアップロードできますが、キーを失ってリーダーボードを使用できなくなるのを防ぐために、アカウントを登録することをお勧めします（アカウントには経験が蓄積されます）。
	* #### 現在、WEBで自分のランドマークを選択できます。
* ### 新しい最適化:
	* #### リーダーボードのGC問題を修正しました。これで、以前のように数千回のGCが発生しません。
	* #### ランドマークのシェーダーを最適化しました。これでランドマークのラグがなくなりました。
	* #### カラフルな名前のシステムを更新しました。現在、カラフルな名前はWEBで完全に作成されるとキャッシュされ、Udonでの繰り返しループがなくなります。
  	* #### 新しいパネルでスクリプトをバインドする際、Start関数でのゲーム読み込み時間を占有しません。
* ### 新しい特典:
	* #### カラフルな名前が無制限になりました。アカウントを登録してプレイヤーをバインドし、カラフルな名前を選ぶことができます。
	* #### レベルシステム（レベル6 - 2セグメントのカラフルな名前、レベル12 - 3セグメントのカラフルな名前、レベル24 - 4セグメントのカラフルな名前）。
* ### 新しいWEB:
	* #### ランドマークとカラフルな名前を設定するための個人パネルを追加しました。
	* #### アップロード回数と履歴を表示する個人情報パネルを追加しました。
	* #### リーダーボード記録機能を追加し、勝率を計算できるようになりました。
	* #### 勝率に基づいて簡易的なランク付けを行い、プレイヤーがスコアだけでなく他の要素にも注目できるようにしました。
	* #### 他言語のWEBが開発中です....
---

## 2. 使い方
### Prefabフォルダを開きます（赤は必須、青はオプション）
![1](https://github.com/user-attachments/assets/24566164-7c7a-4d29-b29f-d012d887821e)
* #### snooker&pyramid&cn8&3c&10b（メインのビリヤードテーブル）を配置します。
* #### TableHook (replica) 2（必須プラグイン）を配置します。
* #### UI-Leaderboard（リーダーボードUI）を配置します。
* #### TagPlug（ランドマークのメイン）（オプションプラグイン）を配置します。

## 次にVRC-VRCSAを上部で探します。
![1](https://github.com/user-attachments/assets/09701d17-b73e-4cee-b834-ca5cb6385cdd)
* ### 最初に「Set Up Pool Table Layers」をクリックします。
* ### 次に「Build Tool」を開きます：
	* #### アカウントにログインします（任意）。
	* #### 上から順にボタンをクリックします。
 	* #### 初期のテスト版であるため、バグがある可能性があります。

## WEBシステムの使い方（初期テスト）
* ### あなたの[個人ページ](https://www.wangqaq.com/PoolBar/Account)でできること：
	* #### 自分のカラフルな名前を設定できます。
	* #### 自分のランドマークを設定できます。
	* #### カラフルな名前を使用したい場合は、まずユーザーをバインドする必要があります。 [マップ](https://vrchat.com/home/world/wrld_d9ac19bc-a8c4-42cd-b712-c66dd813bd8c/info)に行き、User Code SystemからユーザーコードをコピーしてWEBに入力し、バインドをクリックします。
 	*  ![1](https://github.com/user-attachments/assets/b2f3a365-6ebe-452e-9d75-8b798ee98ac2)
* ### [情報板](https://www.wangqaq.com/PoolBar/Information)でできること：
	* #### 自分のシーズン統計、ユーザーレベル、タスクリスト、イベントなどを確認できます。

## 更新計画
	* #### WebGLビリヤードの歴史的ラウンドビュー。
 	* #### 改善されたリーダーボード。
  	* #### 抽象的なビリヤードミニゲーム。
  	* #### 他言語のWEB。

## 特別感謝

### 特別感謝します、皆さん（順不同）：COCO、卵包飯、アンジェロ・ロゼッタ、豚カツ、リアイチなど...
### 特別感謝COCOさん、ロゴ、フロントエンド、UIを作成してくれました（UIとフロントエンドは時間の関係で後でプロジェクトにアップロード予定です）。

## 高画質画像
![1](https://github.com/user-attachments/assets/22d982b4-a50e-420f-8db5-05553483445d)
![1](https://github.com/user-attachments/assets/3ab92dda-c7dc-4ab1-94dd-bce85f6809e2)
![1](https://github.com/user-attachments/assets/90a37503-a4c4-4b7f-936c-17f00c094bec)

![qrcode_1737098291587](https://github.com/user-attachments/assets/ebbfe76c-75b4-4352-b105-5e02ae20ff09)
