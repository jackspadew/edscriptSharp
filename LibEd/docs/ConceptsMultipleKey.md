# Concepts of Multiple Key

## Multiple Key

ハッシュ化、暗号化、ランダマイザに用いる各種`変数`としてのインターフェースを持つ。
DBへの格納を前提とした`バイト配列`としてのインターフェースを持つ。
これらは`IMultipleKeyExchanger`によって表現されている。

## Multiple Key の種類

1. Default MK
2. Initial MK
3. Key Blended MK
4. Stashed MK

### Default Multiple Key

これは`Initial MK`を取得するための`定数`である。
これはプログラムが固定値を持つか、（変更可能であるとしても）環境変数を元に復元される事が想定される。
これは`秘匿された情報ではない`。

### Initial Multiple Key

これはDB作成時にランダムに生成され、`Default MK`で暗号化されDBに格納される。
これは`Default MK`があれば取得可能であり、`秘匿された情報ではない`。
これは初期乱数として`Key Blended MK`を固定化させない効果がある。[^1]

[^1]: 固定化させないことがどの程度秘匿化に役立つかは疑問。必ずしも必要ではない。

### Key Blended MK

これは`Initial MK`と`ユーザー入力秘密鍵`によって復元される。
これはDBには格納されていない。
これはユーザー入力秘密鍵と復元アルゴリズムによって`秘匿化されている`。
これはDBに格納される`Stashed MK`を秘匿化するために必要である。

### Stashed MK

これはランダムに生成され、`Key Blended MK`によって暗号化されDBに格納される。
これは`秘匿化されている`。
暗号化対象毎に`個別の秘密鍵`を用いるために必要な概念である。

`Stashed MK`を`Stashed MK`で暗号化する暗号化チェインのアイディアもある。

## Workerの種類とMultiple Keyの関係表

|           | Default |        Initial        | Key Blended |        Stashed        | Target Data    |
| --------- | :-----: | :-------------------: | :---------: | :-------------------: | -------------- |
| Initial   |   use   | create/stash, extract |             |                       |                |
| ChainZero |         |         read          | create, use | create/stash, extract |                |
| Chain     |         |                       |             |          use          | stash, extract |

- use: 暗号化/復号化に使う。
- create: 生成する。
- create/stash: MKを生成した上で、use鍵で暗号化しそれをDBに格納する。
- stash: use鍵を使用し暗号化、DBに格納する。
- read: データ自体を使うが、直接的には暗号化/復号化には使わない。
- extract: use鍵を使用し復号化する。
- Target Data: 暗号化/復号化を行う対象のデータ

Workerについての定義と説明は割愛するが、MKとの関係でそのWorkerの役割は見える。
