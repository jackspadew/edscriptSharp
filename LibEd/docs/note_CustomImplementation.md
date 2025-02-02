# note for custom implementation

カスタム実装を行う場合のためのメモ

## Usage case without custom implementation

```C#
using LibEd.EdData;

byte[] plainBytes = new byte[]{1,2,3,4};
string indexName = "mydata";

IEdDataLogicFactory logic = new BasicEdDataLogicFactory("example.db", "password");
IEdDataWorker worker = logic.CreateWorker();
worker.Stash(indexName, plainBytes);
byte[] extractedPlainBytes = worker.Extract(indexName);
```

## custom implementation

カスタム実装を行う場合には少なくとも`IEdDataLogicFactory`オブジェクトクラスを実装することになります。
IEdDataLogicFactoryオブジェクトクラスを実装する簡単な方法が用意されています。
それは`EdDataLogicFactoryBase`派生クラスを定義し、抽象メンバを実装する事です。

`BasicEdDataLogicFactory`の実装はカスタム実装の1つと言えます。
コンストラクタ引数を調整する程度の変更であれば、`EdDataLogicFactoryBase`派生クラスを実装するだけで済みます。

カスタム実装の目的が引数の調整に留まらない場合もあります。
その場合は、変更対象のメンバのカスタムクラスを作成した上で、`EdDataLogicFactoryBase`派生クラスのプロパティがカスタムクラスのインスタンスを返す実装を行うようにします。

### Explanation of logic interface

#### IEdDataCryptor

暗号化処理に関連します。
この型のカスタムクラスを作成する際には、ISymmetricEncrypter型のカスタムクラスの作成も必要であると思われます。

#### IEdDataHashCalculator

ハッシュ値処理に関連します。
データベースへの格納時のindex列の値を算出します。
この型のカスタムクラスを作成する際には、IHashCalculator型のカスタムクラスの作成も必要であると思われます。

#### IDatabaseOperator

データベース操作に関連します。

#### IMultipleKeyExchanger

鍵やIVやシード値やSalt等を持ちます。
振る舞いを変更することで影響する範囲は広い。

#### IEdDataWorkerChain

上記の`IEdDataLogicFactory`のクライアントクラスです。
これを変更する前に上記のロジックインターフェースに関するカスタム実装を検討するべき。

#### IEdDataWorkerInitializer

上記の`IEdDataLogicFactory`のクライアントクラスです。
これを変更する前に上記のロジックインターフェースに関するカスタム実装を検討するべき。
