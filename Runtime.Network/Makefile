ASSEMBLY = ../bin/Fmacj.Runtime.Network.dll

FILES = Properties/AssemblyInfo.cs

REFERENCES =

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:library -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
