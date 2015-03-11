namespace ExpertOpinionSharp
{
    /// <summary>
    /// Models a variable to be estimated or a calibration variable whose value is known.
    /// </summary>
    class ExpertVariable {

        /// <summary>
        /// Gets the name of the variable
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpertOpinionSharp.ExpertVariable"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        public ExpertVariable(string name)
        {
            this.Name = name;
        }

    }

    /// <summary>
    /// Models a calibration variable whose value is known.
    /// </summary>
    class CalibrationVariable : ExpertVariable {

        /// <summary>
        /// Gets the true value for the variable.
        /// </summary>
        /// <value>The true value.</value>
        public double TrueValue {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpertOpinionSharp.CalibrationVariable"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="trueValue">True value.</param>
        public CalibrationVariable(string name, double trueValue) : base(name)
        {
            this.TrueValue = trueValue;
        }
    }
}

