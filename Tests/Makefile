BINDIR = ../bin

ASSEMBLY = $(BINDIR)/Fmacj.Tests.dll

FILES =  \
	BusTest.cs \
	ChannelTest.cs \
	ChordTest.cs \
	ForkTest.cs \
	JoinTest.cs \
	ParallelizationFactoryTest.cs \
	Properties/AssemblyInfo.cs \
	ShortcutTest.cs \
	ThreadTimeout.cs \
	TypeValidatorTest.cs


NUNITDIR = ../lib/nunit
NUNITDLL = nunit.framework.dll
NUNITEXE = nunit-console.exe

REFERENCES =  \
	-r:$(BINDIR)/Fmacj.Emitter.dll \
	-r:$(BINDIR)/Fmacj.Framework.dll \
	-r:$(BINDIR)/Fmacj.Runtime.dll \
	-r:$(NUNITDIR)/$(NUNITDLL)

all: $(ASSEMBLY) test

$(ASSEMBLY): $(FILES) 
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:library -debug $(FILES); \

$(BINDIR)/$(NUNITDLL):
	cp -f $(NUNITDIR)/$(NUNITDLL) $(BINDIR)

clean:
	-rm -vf $(ASSEMBLY)*; \
	rm -vf $(BINDIR)/$(NUNITDLL);

new: clean all

compile: $(ASSEMBLY)

test: $(BINDIR)/$(NUNITDLL)
	mono --debug $(NUNITDIR)/$(NUNITEXE) $(ASSEMBLY)

