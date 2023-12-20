using Microsoft.SemanticKernel;
using srbrettle.FinancialFormulas;
using System.Collections.Generic;
using System.ComponentModel;

namespace SemanticKernel.Assistants.Tests.Plugins;

public sealed class FinancialPlugin
{
    [KernelFunction]
    [Description("Calculates the future value of annuity from periodic payment, rate per period and number of periods.")]
    [return: Description("The decimal value for future value of annuity.")]
    public static decimal CalcFutureValueOfAnnuity(
        [Description("The periodic payment.")] double periodicPayment,
        [Description("The decimal value of rate per period (for example 1% => 0.01).")] double ratePerPeriod,
        [Description("The number ofpPeriods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcFutureValueOfAnnuity((decimal)periodicPayment, (decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates the future value of annuity with continuous compounding from cash flow, rate and time.")]
    [return: Description("The decimal value for future value of annuity with continuous compounding.")]
    public static decimal CalcFutureValueOfAnnuityWithContinuousCompounding(
        [Description("The cash flow.")] double cashFlow,
        [Description("The decimal value of the rate (for example 1% => 0.01).")] double rate,
        [Description("The time.")] double time)
    {
        return GeneralFinanceFormulas.CalcFutureValueOfAnnuityWithContinuousCompounding((decimal)cashFlow, (decimal)rate, (decimal)time);
    }

    [KernelFunction]
    [Description("Calculates the number of periods for future value of annuity from future value of annuity, rate and payment.")]
    [return: Description("The decimal value for number of periods for future value of annuity.")]
    public static decimal CalcNumberOfPeriodsForFutureValueOfAnnuity(
        [Description("The future value of annuity.")] double futureValueOfAnnuity,
        [Description("The rate.")] double rate,
        [Description("The payment.")] double payment)
    {
        return GeneralFinanceFormulas.CalcNumberOfPeriodsForFutureValueOfAnnuity((decimal)futureValueOfAnnuity, (decimal)rate, (decimal)payment);
    }

    [KernelFunction]
    [Description("Calculates present value annuity payment from present value, rate per period and number of periods.")]
    [return: Description("The decimal value for present value annuity payment.")]
    public static decimal CalcAnnuityPaymentPresentValue(
        [Description("The present value.")] double presentValue,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcAnnuityPaymentPresentValue((decimal)presentValue, (decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates future value annuity payment from future value, rate per period and number of periods.")]
    [return: Description("The decimal value for future value annuity payment.")]
    public static decimal CalcAnnuityPaymentFutureValue(
        [Description("The future value.")] double futureValue,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcAnnuityPaymentFutureValue((decimal)futureValue, (decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates the number of periods for present value of annuity from present value of annuity, rate and payment.")]
    [return: Description("The decimal value for number of periods for present value of annuity.")]
    public static decimal CalcNumberOfPeriodsForPresentValueOfAnnuity(
        [Description("The present value of annuity.")] double presentValueOfAnnuity,
        [Description("The rate.")] double rate,
        [Description("The payment.")] double payment)
    {
        return GeneralFinanceFormulas.CalcNumberOfPeriodsForPresentValueOfAnnuity((decimal)presentValueOfAnnuity, (decimal)rate, (decimal)payment);
    }

    [KernelFunction]
    [Description("Calculates present value of annuity from periodic payment, rate per period and number of periods.")]
    [return: Description("The decimal value for present value of annuity.")]
    public static decimal CalcPresentValueOfAnnuity(
        [Description("The periodic payment.")] double periodicPayment,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcPresentValueOfAnnuity((decimal)periodicPayment, (decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates the average collection period from receivables turnover.")]
    [return: Description("The decimal value for average collection period.")]
    public static decimal CalcAverageCollectionPeriod(
        [Description("The receivables turnover.")] double receivableTurnover)
    {
        return GeneralFinanceFormulas.CalcAverageCollectionPeriod((decimal)receivableTurnover);
    }

    [KernelFunction]
    [Description("Calculates present value annuity factor from rate per period and number of periods.")]
    [return: Description("The decimal value for present value annuity factor.")]
    public static decimal CalcPresentValueAnnuityFactor(
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcPresentValueAnnuityFactor((decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates present value of annuity due from periodic payment, rate per period and number of periods.")]
    [return: Description("The decimal value for present value of annuity due.")]
    public static decimal CalcPresentValueOfAnnuityDue(
        [Description("The periodic payment.")] double periodicPayment,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcPresentValueOfAnnuityDue((decimal)periodicPayment, (decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates future value of annuity due from periodic payment, rate per period and number of periods.")]
    [return: Description("The decimal value for future value of annuity due.")]
    public static decimal CalcFutureValueOfAnnuityDue(
        [Description("The periodic payment.")] double periodicPayment,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcFutureValueOfAnnuityDue((decimal)periodicPayment, (decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates annuity due payment using present value from present value, rate per period and number of periods.")]
    [return: Description("The decimal value for annuity due payment using present value.")]
    public static decimal CalcAnnuityDuePaymentUsingPresentValue(
        [Description("The present value.")] double presentValue,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcAnnuityDuePaymentUsingPresentValue((decimal)presentValue, (decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates annuity due payment using future value from future value, rate per period and number of periods.")]
    [return: Description("The decimal value for annuity due payment using future value.")]
    public static decimal CalcAnnuityDuePaymentUsingFutureValue(
        [Description("The future value.")] double futureValue,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcAnnuityDuePaymentUsingFutureValue((decimal)futureValue, (decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates doubling time from rate of return.")]
    [return: Description("The decimal value for doubling time.")]
    public static decimal CalcDoublingTime(
        [Description("The rate of return.")] double rateOfReturn)
    {
        return GeneralFinanceFormulas.CalcDoublingTime((decimal)rateOfReturn);
    }

    [KernelFunction]
    [Description("Calculates doubling time with continuous compounding from rate.")]
    [return: Description("The decimal value for doubling time with continuous compounding.")]
    public static decimal CalcDoublingTimeWithContinuousCompounding(
        [Description("The rate.")] double rate)
    {
        return GeneralFinanceFormulas.CalcDoublingTimeWithContinuousCompounding((decimal)rate);
    }

    [KernelFunction]
    [Description("Calculates doubling time for simple interest.")]
    [return: Description("The decimal value for doubling time.")]
    public static decimal CalcDoublingTimeForSimpleInterest(
        [Description("The rate.")] double rate)
    {
        return GeneralFinanceFormulas.CalcDoublingTimeForSimpleInterest((decimal)rate);
    }

    [KernelFunction]
    [Description("Calculates future value from initial investment, rate of return and number of periods.")]
    [return: Description("The decimal value for future value.")]
    public static decimal CalcFutureValue(
        [Description("The initial investment.")] double initialInvestment,
        [Description("The decimal value of rate of return (for example 1% => 0.01)")] double rateOfReturn,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcFutureValue((decimal)initialInvestment, (decimal)rateOfReturn, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates future value with continuous compounding from present value, rate and time.")]
    [return: Description("The decimal value for future value with continuous compounding.")]
    public static decimal CalcFutureValueWithContinuousCompounding(
        [Description("The present value.")] double presentValue,
        [Description("The rate.")] double rate,
        [Description("The time.")] double time)
    {
        return GeneralFinanceFormulas.CalcFutureValueWithContinuousCompounding((decimal)presentValue, (decimal)rate, (decimal)time);
    }

    [KernelFunction]
    [Description("Calculates future value factor from rate per period and number of periods.")]
    [return: Description("The decimal value for future value factor.")]
    public static decimal CalcFutureValueFactor(
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcFutureValueFactor((decimal)ratePerPeriod, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates future value of growing annuity from first payment, rate per period, growth rate and number of periods.")]
    [return: Description("The decimal value for future value of growing annuity.")]
    public static decimal CalcFutureValueOfGrowingAnnuity(
        [Description("The first payment.")] double firstPayment,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The growth rate.")] double growthRate,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcFutureValueOfGrowingAnnuity((decimal)firstPayment, (decimal)ratePerPeriod, (decimal)growthRate, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates growing annuity payment from present value, rate per period, growth rate and number of periods.")]
    [return: Description("The decimal value for growing annuity payment.")]
    public static decimal CalcGrowingAnnuityPaymentFromPresentValue(
        [Description("The present value.")] double presentValue,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The growth rate.")] double growthRate,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcGrowingAnnuityPaymentFromPresentValue((decimal)presentValue, (decimal)ratePerPeriod, (decimal)growthRate, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates growing annuity payment from future value, rate per period, growth rate and number of periods.")]
    [return: Description("The decimal value for growing annuity payment.")]
    public static decimal CalcGrowingAnnuityPaymentFromFutureValue(
        [Description("The future value.")] double futureValue,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The growth rate.")] double growthRate,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcGrowingAnnuityPaymentFromFutureValue((decimal)futureValue, (decimal)ratePerPeriod, (decimal)growthRate, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates present value of growing annuity from first payment, rate per period, growth rate and number of periods.")]
    [return: Description("The decimal value for present value of growing annuity.")]
    public static decimal CalcPresentValueOfGrowingAnnuity(
        [Description("The first payment.")] double firstPayment,
        [Description("The rate per period.")] double ratePerPeriod,
        [Description("The growth rate.")] double growthRate,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcPresentValueOfGrowingAnnuity((decimal)firstPayment, (decimal)ratePerPeriod, (decimal)growthRate, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates present value of growing perpetuity from dividend or coupon at first period, discount rate and growth rate.")]
    [return: Description("The decimal value for present value of growing perpetuity.")]
    public static decimal CalcPresentValueOfGrowingPerpetuity(
        [Description("The dividend or coupon at first period.")] double dividendOrCouponAtFirstPeriod,
        [Description("The discount rate.")] double discountRate,
        [Description("The growth rate.")] double growthRate)
    {
        return GeneralFinanceFormulas.CalcPresentValueOfGrowingPerpetuity((decimal)dividendOrCouponAtFirstPeriod, (decimal)discountRate, (decimal)growthRate);
    }

    [KernelFunction]
    [Description("Calculates number of periods for present value to reach future value from future value, present value and rate per period.")]
    [return: Description("The decimal value for number of periods for present value to reach future value.")]
    public static decimal CalcNumberOfPeriodsForPresentValueToReachFutureValue(
        [Description("The future value.")] double futureValue,
        [Description("The present value.")] double presentValue,
        [Description("The rate per period.")] double ratePerPeriod)
    {
        return GeneralFinanceFormulas.CalcNumberOfPeriodsForPresentValueToReachFutureValue((decimal)futureValue, (decimal)presentValue, (decimal)ratePerPeriod);
    }

    [KernelFunction]
    [Description("Calculates present value of perpetuity from dividend or coupon per period and discount rate.")]
    [return: Description("The decimal value for present value of perpetuity.")]
    public static decimal CalcPresentValueOfPerpetuity(
        [Description("The dividend or coupon per period.")] double dividendOrCouponPerPeriod,
        [Description("The discount rate.")] double discountRate)
    {
        return GeneralFinanceFormulas.CalcPresentValueOfPerpetuity((decimal)dividendOrCouponPerPeriod, (decimal)discountRate);
    }

    [KernelFunction]
    [Description("Calculates present value from cash flow after first period, rate of return and number of periods.")]
    [return: Description("The decimal value for present value.")]
    public static decimal CalcPresentValue(
        [Description("The cash flow after first period.")] double cashFlowAfterFirstPeriod,
        [Description("The rate of return.")] double rateOfReturn,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcPresentValue((decimal)cashFlowAfterFirstPeriod, (decimal)rateOfReturn, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates present value with continuous compounding from cash flow, rate and time.")]
    [return: Description("The decimal value for present value with continuous compounding.")]
    public static decimal CalculatePresentValueWithContinuousCompounding(
        [Description("The cash flow.")] double cashFlow,
        [Description("The rate.")] double rate,
        [Description("The time.")] double time)
    {
        return GeneralFinanceFormulas.CalculatePresentValueWithContinuousCompounding((decimal)cashFlow, (decimal)rate, (decimal)time);
    }

    [KernelFunction]
    [Description("Calculates present value factor from rate of return and number of periods.")]
    [return: Description("The decimal value for present value factor.")]
    public static decimal CalcPresentValueFactor(
        [Description("The rate of return.")] double rateOfReturn,
        [Description("The number of periods.")] double numberOfPeriods)
    {
        return GeneralFinanceFormulas.CalcPresentValueFactor((decimal)rateOfReturn, (decimal)numberOfPeriods);
    }

    [KernelFunction]
    [Description("Calculates rule of 72 from rate expressed as a whole number.")]
    [return: Description("The decimal value for rule of 72.")]
    public static decimal CalcRuleOf72(
        [Description("The rate.")] double rate)
    {
        return GeneralFinanceFormulas.CalcRuleOf72((decimal)rate);
    }

    [KernelFunction]
    [Description("Calculates the rate required to double from length of time.")]
    [return: Description("The decimal value for rate required to double.")]
    public static decimal CalcRateRequiredToDoubleByRuleOf72(
        [Description("The length of time.")] double lengthOfTime)
    {
        return GeneralFinanceFormulas.CalcRateRequiredToDoubleByRuleOf72((decimal)lengthOfTime);
    }
}
