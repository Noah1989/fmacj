ASSEMBLY = ../bin/Fmacj.Runtime.dll

FILES =  \
	Bus.cs \
	Channel.cs \
	ChannelEnumerator.cs \
	ChordManager.cs \
	IChannel.cs \
	Properties/AssemblyInfo.cs

REFERENCES = -r:../bin/Fmacj.Framework.dll

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:library -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
