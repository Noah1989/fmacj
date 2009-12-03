
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Tests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Emitter/bin/Debug/Emitter.dll \
	../Framework/bin/Debug/Framework.dll \
	../Runtime/bin/Debug/Runtime.dll \
	../Components.TaskClient/bin/Debug/Components.TaskClient.dll
BUILD_DIR = bin/Debug

EMITTER_DLL_SOURCE=../Emitter/bin/Debug/Emitter.dll
FRAMEWORK_DLL_SOURCE=../Framework/bin/Debug/Framework.dll
FRAMEWORK_DLL_MDB_SOURCE=../Framework/bin/Debug/Framework.dll.mdb
FRAMEWORK_DLL_MDB=$(BUILD_DIR)/Framework.dll.mdb
RUNTIME_DLL_SOURCE=../Runtime/bin/Debug/Runtime.dll
RUNTIME_DLL_MDB_SOURCE=../Runtime/bin/Debug/Runtime.dll.mdb
RUNTIME_DLL_MDB=$(BUILD_DIR)/Runtime.dll.mdb
EMITTER_DLL_MDB_SOURCE=../Emitter/bin/Debug/Emitter.dll.mdb
EMITTER_DLL_MDB=$(BUILD_DIR)/Emitter.dll.mdb
COMPONENTS_TASKCLIENT_DLL_SOURCE=../Components.TaskClient/bin/Debug/Components.TaskClient.dll
RUNTIME_NETWORK_DLL_SOURCE=../Runtime.Network/bin/Debug/Runtime.Network.dll
RUNTIME_NETWORK_DLL_MDB_SOURCE=../Runtime.Network/bin/Debug/Runtime.Network.dll.mdb
RUNTIME_NETWORK_DLL_MDB=$(BUILD_DIR)/Runtime.Network.dll.mdb
COMPONENTS_TASKCLIENT_DLL_MDB_SOURCE=../Components.TaskClient/bin/Debug/Components.TaskClient.dll.mdb
COMPONENTS_TASKCLIENT_DLL_MDB=$(BUILD_DIR)/Components.TaskClient.dll.mdb
NUNIT_FRAMEWORK_DLL_SOURCE=../lib/nunit/nunit.framework.dll
TESTS_DLL_MDB_SOURCE=bin/Debug/Tests.dll.mdb
TESTS_DLL_MDB=$(BUILD_DIR)/Tests.dll.mdb

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/Tests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Emitter/bin/Release/Emitter.dll \
	../Framework/bin/Release/Framework.dll \
	../Runtime/bin/Release/Runtime.dll \
	../Components.TaskClient/bin/Release/Components.TaskClient.dll
BUILD_DIR = bin/Release

EMITTER_DLL_SOURCE=../Emitter/bin/Release/Emitter.dll
FRAMEWORK_DLL_SOURCE=../Framework/bin/Release/Framework.dll
FRAMEWORK_DLL_MDB=
RUNTIME_DLL_SOURCE=../Runtime/bin/Release/Runtime.dll
RUNTIME_DLL_MDB=
EMITTER_DLL_MDB=
COMPONENTS_TASKCLIENT_DLL_SOURCE=../Components.TaskClient/bin/Release/Components.TaskClient.dll
RUNTIME_NETWORK_DLL_SOURCE=../Runtime.Network/bin/Release/Runtime.Network.dll
RUNTIME_NETWORK_DLL_MDB=
COMPONENTS_TASKCLIENT_DLL_MDB=
NUNIT_FRAMEWORK_DLL_SOURCE=../lib/nunit/nunit.framework.dll
TESTS_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(EMITTER_DLL) \
	$(FRAMEWORK_DLL) \
	$(FRAMEWORK_DLL_MDB) \
	$(RUNTIME_DLL) \
	$(RUNTIME_DLL_MDB) \
	$(EMITTER_DLL_MDB) \
	$(COMPONENTS_TASKCLIENT_DLL) \
	$(RUNTIME_NETWORK_DLL) \
	$(RUNTIME_NETWORK_DLL_MDB) \
	$(COMPONENTS_TASKCLIENT_DLL_MDB) \
	$(NUNIT_FRAMEWORK_DLL) \
	$(TESTS_DLL_MDB)  


RESGEN=resgen2

EMITTER_DLL = $(BUILD_DIR)/Emitter.dll
FRAMEWORK_DLL = $(BUILD_DIR)/Framework.dll
RUNTIME_DLL = $(BUILD_DIR)/Runtime.dll
COMPONENTS_TASKCLIENT_DLL = $(BUILD_DIR)/Components.TaskClient.dll
RUNTIME_NETWORK_DLL = $(BUILD_DIR)/Runtime.Network.dll
NUNIT_FRAMEWORK_DLL = $(BUILD_DIR)/nunit.framework.dll

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

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	../Components.TaskClient/bin/Debug/Components.TaskClient.dll \
	../Emitter/bin/Debug/Emitter.dll \
	../Framework/bin/Debug/Framework.dll \
	../lib/nunit/nunit.framework.dll \
	../Runtime/bin/Debug/Runtime.dll \
	System \
	System.Core

DLL_REFERENCES =  \
	../lib/nunit/nunit.framework.dll

CLEANFILES = $(PROGRAMFILES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,EMITTER_DLL))
$(eval $(call emit-deploy-target,FRAMEWORK_DLL))
$(eval $(call emit-deploy-target,FRAMEWORK_DLL_MDB))
$(eval $(call emit-deploy-target,RUNTIME_DLL))
$(eval $(call emit-deploy-target,RUNTIME_DLL_MDB))
$(eval $(call emit-deploy-target,EMITTER_DLL_MDB))
$(eval $(call emit-deploy-target,COMPONENTS_TASKCLIENT_DLL))
$(eval $(call emit-deploy-target,RUNTIME_NETWORK_DLL))
$(eval $(call emit-deploy-target,RUNTIME_NETWORK_DLL_MDB))
$(eval $(call emit-deploy-target,COMPONENTS_TASKCLIENT_DLL_MDB))
$(eval $(call emit-deploy-target,NUNIT_FRAMEWORK_DLL))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(EMITTER_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FRAMEWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(RUNTIME_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(RUNTIME_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(EMITTER_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(COMPONENTS_TASKCLIENT_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(RUNTIME_NETWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(RUNTIME_NETWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(COMPONENTS_TASKCLIENT_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(NUNIT_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(TESTS_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(EMITTER_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FRAMEWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(RUNTIME_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(RUNTIME_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(EMITTER_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(COMPONENTS_TASKCLIENT_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(RUNTIME_NETWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(RUNTIME_NETWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(COMPONENTS_TASKCLIENT_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(NUNIT_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(TESTS_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local
run: all
	mono --debug ../lib/nunit/nunit-console.exe $(ASSEMBLY) 

include $(top_srcdir)/rules.make
