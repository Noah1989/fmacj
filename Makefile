SUBDIRS = Framework \
	  Runtime \
	  Emitter \
	  Examples.Mandelbrot

TESTDIR = Tests

all:
	@for i in $(SUBDIRS); do \
	(cd $$i; $(MAKE) all) || exit; \
	done;

clean:
	@for i in $(SUBDIRS) $(TESTDIR); do \
	(cd $$i; $(MAKE) clean); done

new: clean all

test: tests-all

tests-all:
	@cd $(TESTDIR); $(MAKE) all 

tests-compile:
	@cd $(TESTDIR); $(MAKE) compile

tests-runonly:
	@cd $(TESTDIR); $(MAKE) test 
