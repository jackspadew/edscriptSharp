# Concepts of Worker Chain

## Worker

`Worker`は次のことを行う主体となるオブジェクトのことを指す。

- 原文に対して暗号化を行い、暗号化済みデータをデータベースに`格納`する。
- データベースから暗号化済みデータを読み込み、復号化を行い原文を`抽出`する。

この一連の振る舞いについて、格納を`Stash`、抽出を`Extract`と呼ぶ。

## Worker Chain

`Worker Chain`は次の事を行うために作られた概念である。

- 各原文に対する暗号化に同一の秘密鍵を使用しない。

この目標のために`原文毎にランダム生成された秘密鍵`を使う必要がある。
そして、ある`Worker`はこの秘密鍵の暗号化/復号化を行う役割を担うべきとなる。
さらに、原文の暗号化/復号化を行う`Worker`はそのWorkerから秘密鍵を受け取る必要があることになる。

まとめると、各Workerには`親子関係`を持つ必要がある。
また、少なくとも次の`Worker`が必要である。

- ChainZero : `パスワード`を使い、秘密鍵のStash/Extractを行う。
- LastChain : 親から受け取った秘密鍵を使い、原文のStash/Extractを行う。

さらにプログラムの設計上必要であると判断したものを加えた結果
次のような`Worker`が設計上の概念として存在する。

- InitialWorker : ChainZeroの親である。InitialWorkerの振る舞いは`Worker Chain`の親子関係の概念に依存しない。
- ChainZero : InitialWorkerから受け取った秘密鍵と`パスワード`を使い、秘密鍵のStash/Extractを行う。
- MiddleWorker : 親から受け取った秘密鍵を使い、子の秘密鍵のStash/Extractを行う。単に`Worker`や`ChainWorker`と呼ぶ場合もある。
- LastChain :  親から受け取った秘密鍵を使い、`原文`のStash/Extractを行う。

## interface

### IEdDataWorker

全ての`Worker`はこれを継承している。

Stash
Extract

### IEdDataWorkerChain

子`ChainWorker`が使う。
`Worker`の種類の判別に用いる。

Depth : `Worker Chain の親子関係`における、自身の階層の位置。
StashChildMultipleKey : 子から呼ばれる。
RegenerateChildMultipleKey : 子から呼ばれる。（子においてindex(byte[])値の衝突が起きた際に、鍵の再生成によってindex値の衝突回避を行う。）
ExtractChildMultipleKey : 子から呼ばれる。

### IEdDataWorkerInitializer

InitialWorkerであるかどうかの判別に用いる。

## implementation

### EdDataInitialWorker

InitialWorker

### EdDataWorkerChainBase

ChainZero, MiddleWorker, LastChain

<u>親Workerから秘密鍵を受け取り、それを使用してStash/Extractを行う。</u>という共通点によりこのような実装になっている。
親がInitialWorkerであればChainZeroとして振る舞い。
クライアントクラスがStash/Extractメソッドを直接呼ぶ時にLastChainとして振る舞う。
