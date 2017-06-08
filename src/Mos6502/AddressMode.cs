namespace Mos6502
{
    public enum AddressMode
    {
        // Name             //     Form          Dissassembly        Description
        Accumulator,        //     A             OPC A               operand is AC
        Absolute,           //     abs           OPC $HHLL           operand is address $HHLL
        AbsoluteXIndexed,   //     abs, X        OPC $HHLL, X        operand is address incremented by X with carry
        AbsoluteYIndexed,   //     abs, Y        OPC $HHLL, Y        operand is address incremented by Y with carry
        Immediate,          //     #             OPC #$BB            operand is byte (BB)
        Implicit,           //     impl          OPC                 operand implied
        Indirect,           //     ind           OPC ($HHLL)         operand is effective address; effective address is value of address
        XIndexedIndirect,   //     X, ind        OPC ($BB, X)        operand is effective zeropage address; effective address is byte (BB) incremented by X without carry
        IndirectYIndexed,   //     ind, Y        OPC ($LL), Y        operand is effective address incremented by Y with carry; effective address is word at zeropage address
        Relative,           //     rel           OPC $BB             branch target is PC + offset (BB), bit 7 signifies negative offset
        ZeroPage,           //     zpg           OPC $LL             operand is of address; address hibyte = zero($00xx)
        ZeroPageXIndexed,   //     zpg, X        OPC $LL, X          operand is address incremented by X; address hibyte = zero($00xx); no page transition
        ZeroPageYIndexed,   //     zpg, Y        OPC $LL, Y          operand is address incremented by Y; address hibyte = zero($00xx); no page transition
    }
}
