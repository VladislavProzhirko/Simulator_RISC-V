@echo off
set CROSS_PREFIX=riscv64-unknown-elf-
set OOCD_ROOT=%~dp0openocd
set OPENOCD_SCRIPTS=%OOCD_ROOT%\tcl
set PATH=%~dp0riscv-unknown-elf-gcc\bin;%~dp0build_tools\bin;%OOCD_ROOT%\src;%PATH%
riscv64-unknown-elf-gcc -c %1.S -DASM -Wa,-march=rv32imfc -march=rv32imfc -mabi=ilp32f -D__riscv_xlen=32 -o %1.o
riscv64-unknown-elf-gcc %1.o -static -fvisibility=hidden -nostdlib -nostartfiles -T link.ld -march=rv32imfc -mabi=ilp32f -o %1.elf
riscv64-unknown-elf-objcopy -O verilog %1.elf %1.hex
del *.o *.elf
exit