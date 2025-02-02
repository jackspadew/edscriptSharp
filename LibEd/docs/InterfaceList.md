# interface list

## interfaces for Adapter of external library

### IHashAlgorithmAdapter

Provides unified means of use to hash algorithm of external libraries.

### ISymmetricAlgorithmAdapter

Provides unified means of use to symmetric key encryption algorithm of external libraries.

### IDatabaseOperator

Provides unified means to use relational database feature of external libraries.

## Cryptographic interface

### ISymmetricEncrypter

The implementation class that performs symmetric key encryption inherits this.

### IHashCalculator

The implementation class that calculate hash values inherits this.

## interface list of EdData

### IMultipleKeyExchanger

The implementation class that behaves as "Multiple Key" inherits this.

### IEdDataWorker

All implementation class that behaves as "Worker" inherits this.

### IEdDataWorkerChain

The implementation class that behaves as a "Worker" of "Worker Chain" inherits this.

### IEdDataWorkerInitializer

All implementation class that behaves as "Initial Worker" of "Worker Chain" inherits this.

### IEdDataLogicFactory

The class that inherits this interface determines X"Worker Chain"XX logic by providing instances used by "Worker".

### IEdDataHashCalculator

The implementation class that calculate hash values with "Multiple Key" inherits this.

### IEdDataCryptor

The implementation class that performs symmetric key encryption with "Multiple Key" inherits this.

## misc

### IListGenerator<T>

Provides a function to generate list of generic type objects.

### IActionExecutor

Provides a means to execution method for given Action array.

### IConverter<T,U>

Provides a conversion function from generic type T to U.
