# Ambiente Ubuntu per TesseractOCR

Per usare la libreria **TesseractOCR** con i pacchetti standard di Ubuntu senza modificare il codice sorgente:

1. **Installare Tesseract e Leptonica**
   ```bash
   sudo apt-get update
   sudo apt-get install -y tesseract-ocr libtesseract-dev libleptonica-dev libc6-dev
   # install .NET SDK 8
   curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
   bash /tmp/dotnet-install.sh --version 8.0.401
   export PATH=$HOME/.dotnet:$PATH
   ```

2. **Creare i collegamenti simbolici richiesti**
   La libreria cerca i file `tesseract55.dll` e `leptonica-1.85.0.dll` nella cartella `x64`. Con l'installazione di Ubuntu i file hanno nomi diversi (ad es. `libtesseract.so.5` e `liblept.so.5`). Creare dei link nella cartella `TesseractOCR/x64`:
   ```bash
   sudo ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /usr/lib/x86_64-linux-gnu/libtesseract55.dll.so
   sudo ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 /usr/lib/x86_64-linux-gnu/libleptonica-1.85.0.dll.so
   sudo ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so
   ```
   Assicurarsi che questi file siano copiati accanto ai binari compilati (ad es. `bin/Debug/net8.0/x64`). Se non vengono copiati automaticamente dal build, copiarli manualmente dopo la compilazione.

3. **Eseguire il programma o i test**
   Dopo la compilazione delle librerie di test, creare i link simbolici nella cartella di output:
   ```bash
   dotnet build TesseractOCR.NetCore31Tests/TesseractOCR.NetCore31Tests.csproj -p:TargetFramework=net8.0
   mkdir -p TesseractOCR.NetCore31Tests/bin/Debug/net8.0/x64
   ln -sf /usr/lib/x86_64-linux-gnu/liblept.so.5 TesseractOCR.NetCore31Tests/bin/Debug/net8.0/x64/libleptonica-1.85.0.dll.so
   ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so.5 TesseractOCR.NetCore31Tests/bin/Debug/net8.0/x64/libtesseract55.dll.so
   ln -sf Configs TesseractOCR.NetCore31Tests/bin/Debug/net8.0/tessdata/configs
   ```

Con questa configurazione la libreria wrapper caricherà le versioni di sistema di Tesseract e Leptonica fornite da Ubuntu.
