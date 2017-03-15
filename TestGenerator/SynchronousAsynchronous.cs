using System;
using System.Collections.Generic;
using KAOSTools.MetaModel;

namespace TestGenerator
{
    public class Tick : Formula
    {
        public override IEnumerable<PredicateReference> PredicateReferences {
            get {
                throw new NotImplementedException();
            }
        }
    }

    public static class AsynchronousExtensions 
    {
        public static Formula ToAsynchronous (this Formula f) {
            if (f is StrongImply) {
                var f2 = ((StrongImply)f);
                return new Globally (new Imply (f2.Left, f2.Right)).ToAsynchronous ();
            }
            if (f is Imply) {
                var f2 = ((Imply)f);
                return new Imply () { Left = f2.Left.ToAsynchronous(), Right = f2.Right.ToAsynchronous() };
            }
            if (f is And) {
                var f2 = ((And)f);
                return new And () { Left = f2.Left.ToAsynchronous(), Right = f2.Right.ToAsynchronous() };
            }
            if (f is Or) {
                var f2 = ((Or)f);
                return new Or () { Left = f2.Left.ToAsynchronous(), Right = f2.Right.ToAsynchronous() };
            }
            if (f is Not) {
                var f2 = ((Not)f);
                return new Not () { Enclosed = f2.Enclosed.ToAsynchronous() };
            }

            if (f is Globally) {
                var f2 = (Globally)f;
                return new Globally () { 
                    Enclosed = new Imply () { 
                        Left = new Tick (), 
                        Right = f2.Enclosed.ToAsynchronous ()
                    }
                };
            }

            if (f is Eventually) {
                var f2 = (Eventually)f;
                return new Eventually () { 
                    Enclosed = new And () { 
                        Left = new Tick (), 
                        Right = f2.Enclosed.ToAsynchronous ()
                    }
                };
            }

            if (f is Next) {
                var f2 = (Next)f;
                return new Next () { Enclosed = 
                    new Or () { 
                        Left = new Until () { 
                            Left = new Not () { 
                                Enclosed = new Tick ()
                            }, 
                            Right = new And () { 
                                Left = new Tick (), 
                                Right = f2.Enclosed.ToAsynchronous ()
                            }
                        },
                        Right = new Globally () { 
                            Enclosed = new Not () { 
                                Enclosed = new Tick ()
                            }
                        }
                    }
                };
            }

            if (f is Until) {
                var f2 = (Until)f;
                return new Until () {
                    Left = new Imply () {
                        Left = new Tick (),
                        Right = f2.Left.ToAsynchronous ()
                    }, 
                    Right = new And () {
                        Left = new Tick (), 
                        Right = f2.Right.ToAsynchronous ()
                    }
                };
            }

            if (f is PredicateReference) {
                return f;
            }

            if (f == null) {
                throw new ArgumentNullException ("Provided formula is null");
            }

            throw new NotImplementedException (f.GetType ().ToString () + " is not supported.");
        }
    }
}

