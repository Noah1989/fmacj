BINDIR = ../bin

ASSEMBLY = $(BINDIR)/Fmacj.Tests.dll

FILES =  \
	BusTest.cs \
	ChannelEnumerableTest.cs \
	ChannelResolver.cs \
	ChannelTest.cs \
	ChordTest.cs \
	DisposeTest.cs \
	DistributionTest.cs \
	EnumerableChannelTest.cs \
	ForkTest.cs \
	JoinTest.cs \
	ParallelizationFactoryTest.cs \
	Properties/AssemblyInfo.cs \
	ShortcutTest.cs \
	ThreadTimeout.cs \
	TypeValidatorTest.cs \
	YieldTest.cs


NUNITDIR = ../lib/nunit
NUNITDLL = nunit.framework.dll
NUNITEXE = nunit-console.exe

REFERENCES =  \
	-r:$(BINDIR)/Fmacj.Components.TaskClient.dll \
	-r:$(BINDIR)/Fmacj.Emitter.dll \
	-r:$(BINDIR)/Fmacj.Framework.dll \
	-r:$(BINDIR)/Fmacj.Runtime.dll \
	-r:$(NUNITDIR)/$(NUNITDLL) \
	-r:../bin/Fmacj.Components.TaskClient.dll \
	-r:../bin/Fmacj.Emitter.dll \
	-r:../bin/Fmacj.Framework.dll \
	-r:../bin/Fmacj.Runtime.dll \
	-r:../lib/nunit/nunit.framework.dll

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
	-mono $(BINDIR)/Fmacj.Executables.DistributionServer.exe & echo $$! > ds.pid
	mono --debug $(NUNITDIR)/$(NUNITEXE) $(ASSEMBLY) ; TEST_RESULT=$$? ; \
	kill `cat ds.pid` ; rm ds.pid ; \
	[[ $$TEST_RESULT == 0 ]]

