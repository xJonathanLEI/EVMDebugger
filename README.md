# EVM Debugger

This project aims to build the most feature-rich and user-friendly GUI debugger for Ethereum Virtual Machine (EVM) assembly code.

## The Need for an Assembly Debugger

Smart contracts were designed to be verifiable and trust-less. However, the trustless-ness is only true when the source code (in Solidity) is available, which is *not* always the case.

Reverse engineering solutions like [Porosity](https://github.com/comaeio/porosity) exists. But they don't work well enough to identify scam contracts.

Ultimately, contracts are executed in byte-codes. It is thus always possible to debug Ethereum contracts using byte-codes. Yet I've been searching for quite a while and failed to find a debugger that is nice to use (with automated testing, etc.). So I decided to write my own.

## Features

The debugger supports (well, will support) a wide of features including:

- Import contract code from address directly via Etherscan API
- Setup transaction information with transaction hash via Etherscan API
- Automated testing for different input values (sender, gas, data, etc.)
- 

## Dependency

The EVM project uses the `SHA3` NuGet library for Keccak-256 hashing.

## Environment

This project is originally developed in Windows 10 Pro 16299.192 with Microsoft Visual Studio Community 2017 Version 15.5.3.

In the future, the project may be migrated to be cross platform.

## Known Issues

Issues in this list are to be fixed in a future update. For any issues or bugs outside this list, please submit a GitHub issue (or a pull request directly).

- No gas consumption calculations (so never out of gas)
- External call not supported
- No step-back support

## Acknowledgement

No. I don't like the Ethereum yellow paper. It's so hard to read (at least for me) that it makes me more confused after reading. The implementation of EVM in this project makes extensive reference to that of [cpp-ethereum](https://github.com/ethereum/cpp-ethereum).

## Contribution

Open discussions are welcomed! Feel free to submit an issue for any problem regarding this project. It would be even better to submit pull request to improve the debugger.

Or you can contract me via email: xJonathan@outlook.com
