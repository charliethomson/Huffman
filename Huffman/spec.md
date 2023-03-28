# HEC Binary File Format

Little endian

- 0x0000: Sentinel $HEC (`0x24484543`)
- 0x0008: u32 TREE_SIZE - The number of bytes in the encoded tree
- 0x000C - 0x000C + TREE_SIZE - Encoded tree - see below
- 0x000C + TREE_SIZE: u32 DATA_SIZE - The number of bytes in the encoded data
- ^ + 1 - LAST_BYTE_PADDING - The number of bits (0-8) padding the _end_ of the last byte in DATA (0-8)
- 0x000C + TREE_SIZE + 0x0004 - 0x000C + TREE_SIZE + 0x0004 + DATA_SIZE: HEC encoded data - regular huffman coded stuff

## Tree
node[] -> 1 indexed, 0 is null, 1 is root, root is first

Node encoding (relative to node)
- 0x00: u8 LEFT_INDEX - The index in the node array that corresponds to the left child of this node
- 0x01: u8 RIGHT_INDEX - The index in the node array that corresponds to the right child of this node
- 0x02: char ITEM - The ascii (u8) char pointed at by this node, \0 means the node is a branch rather than a leaf
