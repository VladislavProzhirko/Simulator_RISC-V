OUTPUT_ARCH( "riscv" )

MEMORY {
  RAM (rwx) : ORIGIN = 0x0, LENGTH = 64K
}

SECTIONS {

  /* code segment */
  .text  : {
    *crt.o(.text .text.*)
    *(.text .text.*)
    *(sc_test_section)
	. = ALIGN(0x1000);
    PROVIDE(__TEXT_END__ = .);
  } >RAM 

  /* data segment */
  .data ALIGN(0x1000): {
	*(.data .data.*)
	. = ALIGN(0xF100);
	PROVIDE(__DATA_END__ = .);
  } >RAM

}